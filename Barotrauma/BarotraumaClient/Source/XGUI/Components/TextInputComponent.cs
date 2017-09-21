using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using EventInput;
using Microsoft.Xna.Framework.Graphics;
#if WINDOWS
using System.Windows;
#endif

namespace Barotrauma.XGUI
{
    public class TextInputComponent : TextComponent, IKeyboardSubscriber
    {
        private static TextInputComponent selected = null;

        public bool Selected
        {
            get
            {
                return selected == this;
            }
            set
            {
                if (value)
                {
                    selected = this;
                }
                else if (selected == this)
                {
                    selected = null;
                }
            }
        }

        private void Select()
        {
            GetXGUI().KeyboardDispatcher.Subscriber = this;
        }

        public int selectingState = 0;
        public int selectStart;
        public int caretPos;

        public TextInputComponent(GUIEntity creator, XElement elem) : base(creator, elem) { }

        public override void Update(float deltaTime)
        {
            //We don't call base.Update because we don't expect input boxes to have children

            GUIObject parentObject = GetParentObject();

            if (parentObject.isMouseOn && PlayerInput.LeftButtonHeld())
            {
                Vector2 size = font.MeasureString(str);
                Rectangle targetRect = GUIRectangle.ScaleToXNARect(GUIRectangle.ScaleToOuterRect(rect, owner.GetScaledRect()), new Rectangle(0, 0, GameMain.GraphicsWidth, GameMain.GraphicsHeight));
                Vector2 pos = new Vector2(targetRect.X, targetRect.Y);
                
                if (halign == "center")
                {
                    pos.X = targetRect.X + targetRect.Width / 2 - size.X / 2;
                }
                else if (halign == "right")
                {
                    pos.X = targetRect.X + targetRect.Width - size.X;
                }
                if (valign == "center")
                {
                    pos.Y = targetRect.Y + targetRect.Height / 2 - size.Y / 2;
                }
                else if (halign == "bottom")
                {
                    pos.Y = targetRect.Y + targetRect.Height - size.Y;
                }

                pos.X = (float)Math.Round(pos.X); pos.Y = (float)Math.Round(pos.Y);
                Select();

                float charWidth = font.MeasureString("T").X;
                
                int newCaretPos = 0;
                for (int i=1;i<=str.Length;i++)
                {
                    if (font.MeasureString(str.Substring(0, i)).X > (PlayerInput.MousePosition.X - pos.X)) break;
                    newCaretPos = i;
                }

                if (selectingState == 0 || PlayerInput.LeftButtonDown())
                {
                    if (PlayerInput.KeyDown(Keys.LeftShift) || PlayerInput.KeyDown(Keys.RightShift))
                    {
                        if (selectingState != 2)
                        {
                            selectStart = caretPos;
                            selectingState = 2;
                        }
                    }
                    else
                    {
                        selectingState = 1;
                        selectStart = newCaretPos;
                    }
                }
                else if (newCaretPos != selectStart || selectingState == 2)
                {
                    selectingState = 2;
                }
                caretPos = newCaretPos;
            }
            else if (selectingState == 1)
            {
                selectingState = 0;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            //We don't call base.Draw because we don't expect input boxes to have children
            //We also need to draw the caret

            Vector2 size = font.MeasureString(str);
            Rectangle targetRect = GUIRectangle.ScaleToXNARect(GUIRectangle.ScaleToOuterRect(rect, owner.GetScaledRect()), new Rectangle(0, 0, GameMain.GraphicsWidth, GameMain.GraphicsHeight));
            Vector2 pos = new Vector2(targetRect.X, targetRect.Y);

            Vector2 caretSize = font.MeasureString(str.Substring(0, caretPos));

            if (halign == "center")
            {
                pos.X = targetRect.X + targetRect.Width / 2 - size.X / 2;
            }
            else if (halign == "right")
            {
                pos.X = targetRect.X + targetRect.Width - size.X;
            }
            if (valign == "center")
            {
                pos.Y = targetRect.Y + targetRect.Height / 2 - size.Y / 2;
            }
            else if (halign == "bottom")
            {
                pos.Y = targetRect.Y + targetRect.Height - size.Y;
            }

            pos.X = (float)Math.Round(pos.X); pos.Y = (float)Math.Round(pos.Y);

            if (selectingState==2)
            {
                Vector2 selectSize = font.MeasureString(str.Substring(0, selectStart));

                int startX = (int)Math.Min(caretSize.X, selectSize.X);
                int endX = (int)Math.Max(caretSize.X, selectSize.X);

                Rectangle selectRect = new Rectangle((int)pos.X+startX, (int)pos.Y, endX-startX, (int)size.Y);

                GetXGUI().DrawRectangle(spriteBatch, selectRect, new Color(0.3f, 0.3f, 0.9f), true);
            }

            GetXGUI().DrawLine(spriteBatch, pos + new Vector2(caretSize.X, 0), pos + caretSize, Color.White);

            font.DrawString(spriteBatch, str, pos, Color.White);
        }

        private void EraseSelectedText()
        {
            if (selectingState != 2) return;

            int start = Math.Min(caretPos,selectStart);
            int end = Math.Max(caretPos, selectStart);

            str = str.Substring(0, start) + str.Substring(end);

            caretPos = start;

            selectingState = 0;
        }

        public void ReceiveTextInput(char inputChar)
        {
            EraseSelectedText();

            str = str.Substring(0, caretPos) + inputChar + str.Substring(caretPos);
            caretPos++;
        }
        
        public void ReceiveTextInput(string text)
        {
            EraseSelectedText();

            text = text.Replace("\r", "");
            text = text.Replace("\n", "");
            str = str.Substring(0, caretPos) + text + str.Substring(caretPos);
            caretPos += text.Length;
        }
        public void ReceiveCommandInput(char command)
        {
            int startPos; int endPos;
            switch (command)
            {
                case '\b': //backspace
                    if (selectingState == 2)
                    {
                        EraseSelectedText();
                    }
                    else if (str.Substring(0, caretPos).Length > 0)
                    {
                        str = str.Substring(0, caretPos - 1) + str.Substring(caretPos);
                        caretPos--;
                    }
                    break;
#if WINDOWS
                case (char)0x03: //copy
                    if (selectingState != 2) break;
                    startPos = Math.Min(caretPos, selectStart);
                    endPos = Math.Max(caretPos, selectStart);
                    Clipboard.SetText(str.Substring(startPos, endPos - startPos));
                    break;
                case (char)0x18: //cut
                    if (selectingState != 2) break;
                    startPos = Math.Min(caretPos, selectStart);
                    endPos = Math.Max(caretPos, selectStart);
                    Clipboard.SetText(str.Substring(startPos, endPos - startPos));
                    EraseSelectedText();
                    break;
#endif
                default:
                    //DebugConsole.NewMessage(((int)command).ToString());
                    break;
            }
        }

        public void ReceiveSpecialInput(Keys key)
        {
            switch (key)
            {
                case Keys.Left:
                    if (PlayerInput.KeyDown(Keys.LeftShift) || PlayerInput.KeyDown(Keys.RightShift))
                    {
                        if (selectingState != 2)
                        {
                            selectingState = 2;
                            selectStart = caretPos;
                        }
                    }
                    else
                    {
                        selectingState = 0;
                    }
                    caretPos--;
                    caretPos = MathHelper.Clamp(caretPos, 0, str.Length);
                    break;
                case Keys.Right:
                    if (PlayerInput.KeyDown(Keys.LeftShift) || PlayerInput.KeyDown(Keys.RightShift))
                    {
                        if (selectingState != 2)
                        {
                            selectingState = 2;
                            selectStart = caretPos;
                        }
                    }
                    else
                    {
                        selectingState = 0;
                    }
                    caretPos++;
                    caretPos = MathHelper.Clamp(caretPos, 0, str.Length);
                    break;
            }
        }
    }
}
