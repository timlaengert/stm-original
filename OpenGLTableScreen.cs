using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
//using Tao.FreeType;
//using Tao.FtGl;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Platform.Windows;

namespace ShootClient
{
    partial class OpenGLTableScreen : Form
    {
        const bool SHOW_FRAMERATE = false;

        internal delegate void StandardHandler(object[] args);
        internal delegate void NoParamsHandler();

        //static SimpleOpenGlControl canvas = new SimpleOpenGlControl();
        static GLControl canvas = new GLControl();

        static BoundingBox rootBB;

        static float WINDOW_ASPECT_RATIO;
        static float WINDOW_MIN_X;
        static float WINDOW_MAX_X;
        static float WINDOW_MIN_Y;
        static float WINDOW_MAX_Y;

        static float SIDEBAR_WIDTH;
        static float SIDEBAR_HEIGHT;

        static float PLAY_AREA_WIDTH;
        static float PLAY_AREA_HEIGHT;
        static float PLAY_AREA_CENTRE_X;
        static float PLAY_AREA_CENTRE_Y;
        const float EXTRA_BUFFER_BETWEEN_TABLE_AND_CARDS = 0.1f;
        const float HALF_TABLE_WIDTH = 1.0f;

        static float[] PLAYED_CARD_X = new float[6];
        static float[] PLAYED_CARD_Y = new float[6];
        static OpenGLCard[] PLAYED_CARDS = new OpenGLCard[6];
        static OpenGLCard WINNING_CARD = null;

        const float MAX_CARDS_IN_HAND = 10;
        const float RELATIVE_HAND_CARD_BUFFER_WIDTH = 0.15f;
        const float HAND_CARD_SCALE_FACTOR = 0.8f;
        static float HAND_CARD_WIDTH;
        static float HAND_CARD_HEIGHT;
        static List<OpenGLCard> HAND_CARDS = new List<OpenGLCard>();
        static int SELECTED_CARD_INDEX = -1;

        static OpenGLLabel[] PLAYER_LABELS = new OpenGLLabel[6];

        static OpenGLLabel[] SCORE_LABELS = new OpenGLLabel[2];
        static OpenGLLabel[] TRICK_LABELS = new OpenGLLabel[2];
        static OpenGLLabel CALL_LABEL;

        static OpenGLToggle[] BID_TOGGLES = new OpenGLToggle[15];
        static int tentativeTricks;
        static Trump tentativeTrump;
        static OpenGLToggle activeTrickToggle = null;
        static OpenGLToggle activeTrumpToggle = null;
        static OpenGLButton bidButton;
        static string[] playerSubText = new string[] { string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty };
        static string[] playerSubSubText = new string[] { string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty };
        static string centreText = string.Empty;
        static string upperText = string.Empty;

        Texture playAreaBackground, table;

        System.Drawing.Text.PrivateFontCollection PFC = new System.Drawing.Text.PrivateFontCollection();
        static OpenTK.Graphics.TextPrinter textPrinter = new OpenTK.Graphics.TextPrinter(OpenTK.Graphics.TextQuality.High);
        static Font font;
        //static FtGl.FTFont font;
        
        const float FONT_SIZE_MODIFIER = 0.1f / 34f; // approximate mapping is 0.1 Window units = 34 font points.  Update if neccessary.
        const int FONT_RESOLUTION = 36;

        RichTextBox chatInputBox = new RichTextBox();
        RichTextBox chatOutputBox = new RichTextBox();
        static FontFamily fontFamily;

        static Bool hightlightLegalCards = new Bool(true);
        static Bool readyToPlay = new Bool(false);
        static Player.PlayerStatus myStatus = Player.PlayerStatus.LOOKING_FOR_SEAT;

        static BoundingBox depressed = null;
        static bool mouseIsDown = false;

        OpenGLCheckbox readyCheckBox;
        OpenGLCheckbox highlightCardsCheckBox;

        OpenGLButton aboutButton;

        static List<OpenGLControl> drawList;
        static List<OpenGLControl> animationList;
        static System.Threading.Timer animationTimer;
        static int frameCounter = 0;
        static float currentFramerate = 0;
        static DateTime startedTiming = DateTime.Now;
        static TimeSpan elapsedTime = TimeSpan.Zero;

        const int FRAMERATE = 60;

        public OpenGLTableScreen()
        {
            InitializeComponent();
            //foreach (Control control in this.Controls) control.Visible = false;
            this.Controls.Add(canvas);
            this.Controls.Add(chatInputBox);
            this.Controls.Add(chatOutputBox);
            canvas.Dock = System.Windows.Forms.DockStyle.Fill;
            canvas.Paint += new PaintEventHandler(PaintHandler);
            canvas.MouseClick += new MouseEventHandler(mouseClickHandler);
            canvas.MouseDown += new MouseEventHandler(mouseDownHandler);
            canvas.MouseUp += new MouseEventHandler(mouseUpHandler);
            canvas.MouseMove += new MouseEventHandler(mouseMoveHandler);
            canvas.KeyPress += new KeyPressEventHandler(keyPressHandler);
            canvas.LostFocus += new EventHandler(lostFocusHandler);
            //canvas.InitializeContexts();

            WINDOW_ASPECT_RATIO = (float)canvas.Width / (float)canvas.Height;
            WINDOW_MAX_X = WINDOW_ASPECT_RATIO;
            WINDOW_MIN_X = -WINDOW_MAX_X;
            WINDOW_MAX_Y = 1;
            WINDOW_MIN_Y = -WINDOW_MAX_Y;

            rootBB = new BoundingBox(WINDOW_MIN_X, WINDOW_MIN_Y, WINDOW_MAX_X, WINDOW_MAX_Y, null, null, null, null, null);

            //GL.MatrixMode(Gl.GL_PROJECTION);
            GL.MatrixMode(MatrixMode.Projection);
            //GL.LoadIdentity();
            GL.LoadIdentity();
            GL.Viewport(0, 0, canvas.Width, canvas.Height);
            //GL.Ortho(WINDOW_MIN_X, WINDOW_MAX_X, WINDOW_MIN_Y, WINDOW_MAX_Y, -1, 1);
            GL.Ortho(WINDOW_MIN_X, WINDOW_MAX_X, WINDOW_MIN_Y, WINDOW_MAX_Y, -1, 1);

            //GL.MatrixMode(Gl.GL_TEXTURE);
            GL.MatrixMode(MatrixMode.Texture);
            //GL.Rotatef(180.0f, 0.0f, 0.0f, 1.0f);
            GL.Rotate(180, 0, 0, 1);
            //GL.Scalef(-1.0f, 1.0f, 1.0f);
            GL.Scale(-1, 1, 1);

            //GL.MatrixMode(Gl.GL_MODELVIEW);
            GL.MatrixMode(MatrixMode.Modelview);
            //GL.LoadIdentity();
            GL.LoadIdentity();
            //GL.ClearColor(0.0f, 0.0f, 0.0f, 0.0f);
            GL.ClearColor(0, 0, 0, 0);
            //GL.Clear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            //GL.Enable(Gl.GL_BLEND);
            GL.Enable(EnableCap.Blend);
            //GL.BlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            //TexUtil.InitTexturing();

            playAreaBackground = Texture.LoadTexture("background.png", 256, 257); // 50, 1028); // 1024, 768);
            table = Texture.LoadTexture("table.png", 847, 257); // 723, 219); // 1694, 514);
            OpenGLLabel.Initialize();
            OpenGLCheckbox.Initialize();
            OpenGLToggle.Initialize();
            OpenGLButton.Initialize();
            //cardTexture = Texture.LoadTexture("CardMaster.png", 75, 107);
            //highlightTexture = Texture.LoadTexture("highlight.png", 95, 127);

            SIDEBAR_HEIGHT = WINDOW_MAX_Y - WINDOW_MIN_Y;
            SIDEBAR_WIDTH = SIDEBAR_HEIGHT * SideBar.SIDEBAR_ASPECT_RATIO;
            PLAY_AREA_WIDTH = WINDOW_MAX_X - WINDOW_MIN_X - SIDEBAR_WIDTH;
            PLAY_AREA_CENTRE_X = WINDOW_MIN_X + PLAY_AREA_WIDTH / 2;
            HAND_CARD_WIDTH = PLAY_AREA_WIDTH / (MAX_CARDS_IN_HAND * (1 + RELATIVE_HAND_CARD_BUFFER_WIDTH)) * HAND_CARD_SCALE_FACTOR;
            HAND_CARD_HEIGHT = HAND_CARD_WIDTH / OpenGLCard.AspectRatio;
            PLAY_AREA_HEIGHT = WINDOW_MAX_Y - WINDOW_MIN_Y - HAND_CARD_HEIGHT - EXTRA_BUFFER_BETWEEN_TABLE_AND_CARDS;
            PLAY_AREA_CENTRE_Y = WINDOW_MAX_Y - PLAY_AREA_HEIGHT / 2;

            PFC.AddFontFile("Tahoma.ttf");
            fontFamily = PFC.Families[0];
            //font = new FtGl.FTGLPixmapFont("vera.ttf");
            //font.FaceSize(40, FONT_RESOLUTION);
            //font = new OpenTK.Graphics.TextureFont(fontFamily, 10);
            font = new System.Drawing.Font(fontFamily, 10);

            InitializePlayArea();
            InitializeSidebar();

            chatInputBox.SuspendLayout();
            chatInputBox.KeyPress += new KeyPressEventHandler(chatKeyPressHandler);
            chatInputBox.Multiline = false;
            chatInputBox.ForeColor = Color.White;
            chatInputBox.BackColor = Color.Black;
            chatInputBox.BorderStyle = BorderStyle.None;
            chatInputBox.Font = new Font(fontFamily, 12);
            chatInputBox.Height = chatInputBox.PreferredHeight;
            chatInputBox.Width = sizeToPixels(SIDEBAR_WIDTH - SideBar.INDENT * 2, 0).Width;
            chatInputBox.Location = coordsToPixels(WINDOW_MAX_X - SIDEBAR_WIDTH + SideBar.INDENT * 1.5f,
                WINDOW_MIN_Y + SideBar.CHAT_BODY_Y1 + SideBar.INDENT * 0.5f);
            chatInputBox.Location = new Point(chatInputBox.Location.X, chatInputBox.Location.Y - chatInputBox.Size.Height); // adjust for font size
            chatInputBox.Visible = true;
            chatInputBox.Text = string.Empty;
            chatInputBox.ResumeLayout();
            chatInputBox.BringToFront();

            chatOutputBox.SuspendLayout();
            chatOutputBox.ForeColor = Color.White;
            chatOutputBox.BackColor = Color.Black;
            chatOutputBox.BorderStyle = BorderStyle.None;
            chatOutputBox.Font = new Font(fontFamily, 12);
            //chatOutputBox.Height = chatOutputBox.PreferredHeight;
            chatOutputBox.Location = new Point(chatInputBox.Location.X,
                coordsToPixels(0, WINDOW_MIN_Y + SideBar.CHAT_BODY_Y2 - SideBar.INDENT * 0.5f).Y);
            chatOutputBox.Height = chatInputBox.Location.Y - chatOutputBox.Location.Y - 5;
            chatOutputBox.Width = sizeToPixels(SIDEBAR_WIDTH - SideBar.INDENT * 2, 0).Width;
            chatOutputBox.Visible = true;
            chatOutputBox.Text = string.Empty;
            chatOutputBox.ReadOnly = true;
            chatOutputBox.ResumeLayout();
            chatOutputBox.BringToFront();

            animationTimer = new System.Threading.Timer(new System.Threading.TimerCallback(UpdateAnimations));
            //GL.ShadeModel(Gl.GL_FLAT);
            //GL.Enable(Gl.GL_DEPTH_TEST);
            //GL.Enable(Gl.GL_CULL_FACE);
            //Glu.gluOrtho2D(0, this.Width, 0, this.Height);

            //SetView(this.Height, this.Width);
        }

        /// <summary>
        /// Delete all textures used.
        /// </summary>
        public void CleanUp()
        {
            OpenGLButton.DeleteTextures();
            OpenGLCard.DeleteTextures();
            OpenGLCheckbox.DeleteTextures();
            OpenGLLabel.DeleteTextures();
            OpenGLToggle.DeleteTextures();
            playAreaBackground.DeleteTextureData();
            table.DeleteTextureData();
        }

        /// <summary>
        /// Paint the control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PaintHandler(object sender, PaintEventArgs e)
        {
            RenderScene();
        }

        /// <summary>
        /// Draws a frame.
        /// </summary>
        private void RenderScene()
        {
            // Clear Screen And Depth Buffer
            //GL.Clear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            //GL.PushMatrix();
            drawPlayArea();
            drawPlayerLabels();
            //drawSideBar();
            ////drawHandCardsOnTable();
            //drawHandCards();
            //drawPlayedCards();
            //lock (drawList) foreach (OpenGLControl control in drawList) control.Draw();
            //if (SHOW_FRAMERATE) frameCounter++;
            //// do our drawing here
            //// initial viewing transformation
            ////const float viewAngle = 103.0f;
            ////GL.Rotatef(viewAngle, 1.0f, 0.2f, 0.0f);
            ////GL.PopMatrix();
            ////GL.Flush();
            canvas.SwapBuffers();
        }

        private void SetView(int height, int width)
        {
            //// Set viewport to window dimensions.
            //GL.Viewport(0, 0, width, height);
            //// Reset projection matrix stack
            //GL.MatrixMode(Gl.GL_PROJECTION);
            //GL.LoadIdentity();
            //const float nRange = 80.0f;
            //// Prevent a divide by zero
            //if (height == 0)
            //{
            //    height = 1;
            //}
            //// Establish clipping volume (left, right, bottom,
            //// top, near, far)
            //if (width <= height)
            //{
            //    GL.Ortho(-nRange, nRange, -nRange * height / width,
            //    nRange * height / width, -nRange, nRange);
            //}
            //else
            //{
            //    GL.Ortho(-nRange * width / height,
            //    nRange * width / height,
            //    -nRange, nRange, -nRange, nRange);
            //}
            //// reset modelview matrix stack
            //GL.MatrixMode(Gl.GL_MODELVIEW);
            //GL.LoadIdentity();
        }

        /// <summary>
        /// Handles the resizing of the form.
        /// </summary>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            //SetView(this.Height, this.Width);
        }

        private Point coordsToPixels(float x, float y)
        {
            float normalizedX = (x - WINDOW_MIN_X) / (WINDOW_MAX_X - WINDOW_MIN_X);
            float normalizedY = 1 - (y - WINDOW_MIN_Y) / (WINDOW_MAX_Y - WINDOW_MIN_Y);
            int pixelX = (int)Math.Round(normalizedX * this.ClientSize.Width);
            int pixelY = (int)Math.Round(normalizedY * this.ClientSize.Height);
            return new Point(pixelX, pixelY);
        }

        private Size sizeToPixels(float width, float height)
        {
            int pixelWidth = (int)Math.Round(width / (WINDOW_MAX_X - WINDOW_MIN_X) * this.ClientSize.Width);
            int pixelHeight = (int)Math.Round(height / (WINDOW_MAX_Y - WINDOW_MIN_Y) * this.ClientSize.Height);
            return new Size(pixelWidth, pixelHeight);
        }

        public SizeF sizetoCoords(SizeF size)
        {
            float coordsWidth = (WINDOW_MAX_X - WINDOW_MIN_X) * size.Width / this.ClientSize.Width;
            float coordsHeight = (WINDOW_MAX_Y - WINDOW_MIN_Y) * size.Height / this.ClientSize.Height;
            return new SizeF(coordsWidth, coordsHeight);
        }

        private void drawPlayArea()
        {
            bool USE_BACKGROUND_TEXTURE = true;
            float HALF_TABLE_HEIGHT = HALF_TABLE_WIDTH / table.AspectRatio;
            
            //GL.Color4(1, 1, 1, 1);
            GL.Color4(Color.White);
            // draw background
            if (USE_BACKGROUND_TEXTURE)
            {
                //GL.BindTexture(TextureTarget.Texture2D, playAreaBackground.TextureID);
                GL.BindTexture(TextureTarget.Texture2D, playAreaBackground.TextureID);
                //GL.Begin(BeginMode.Quads);
                GL.Begin(BeginMode.Quads);
                //GL.TexCoord2(0, 0); GL.Vertex2(WINDOW_MIN_X, WINDOW_MIN_Y);
                GL.TexCoord2(0, 0); GL.Vertex2(WINDOW_MIN_X, WINDOW_MIN_Y);
                //GL.TexCoord2(0, playAreaBackground.TextureHeight); GL.Vertex2(WINDOW_MIN_X, WINDOW_MAX_Y);
                GL.TexCoord2(0, playAreaBackground.TextureHeight); GL.Vertex2(WINDOW_MIN_X, WINDOW_MAX_Y);
                //GL.TexCoord2(playAreaBackground.TextureWidth, playAreaBackground.TextureHeight); GL.Vertex2(WINDOW_MAX_X, WINDOW_MAX_Y);
                GL.TexCoord2(playAreaBackground.TextureWidth, playAreaBackground.TextureHeight); GL.Vertex2(WINDOW_MAX_X, WINDOW_MAX_Y);
                //GL.TexCoord2(playAreaBackground.TextureWidth, 0); GL.Vertex2(WINDOW_MAX_X, WINDOW_MIN_Y);
                GL.TexCoord2(playAreaBackground.TextureWidth, 0); GL.Vertex2(WINDOW_MAX_X, WINDOW_MIN_Y);
                //GL.End();
                GL.End();
            }
            else
            {
                //GL.Disable(TextureTarget.Texture2D);
                GL.Disable(EnableCap.Texture2D);
                //GL.Begin(BeginMode.Quads);
                GL.Begin(BeginMode.Quads);
                //GL.Color3f(0.15f, 0.15f, 0.15f);
                GL.Color3(0.15, 0.15, 0.15);
                //GL.Vertex2(WINDOW_MIN_X, WINDOW_MAX_Y);
                GL.Vertex2(WINDOW_MIN_X, WINDOW_MAX_Y);
                //GL.Vertex2(WINDOW_MAX_X, WINDOW_MAX_Y);
                GL.Vertex2(WINDOW_MAX_X, WINDOW_MAX_Y);
                //GL.Color3f(0.35f, 0.35f, 0.35f);
                GL.Color3(0.35, 0.35, 0.35);
                //GL.Vertex2(WINDOW_MAX_X, WINDOW_MIN_Y);
                GL.Vertex2(WINDOW_MAX_X, WINDOW_MIN_Y);
                //GL.Vertex2(WINDOW_MIN_X, WINDOW_MIN_Y);
                GL.Vertex2(WINDOW_MIN_X, WINDOW_MIN_Y);
                //GL.End();
                GL.End();
                //GL.Enable(TextureTarget.Texture2D);
                GL.Enable(EnableCap.Texture2D);
                //GL.Color3f(1f, 1f, 1f);
                GL.Color3(1, 1, 1);
            }

            // draw table
            GL.BindTexture(TextureTarget.Texture2D, table.TextureID);
            GL.Begin(BeginMode.Quads);
            GL.TexCoord2(0, 0); GL.Vertex2(PLAY_AREA_CENTRE_X - HALF_TABLE_WIDTH, PLAY_AREA_CENTRE_Y - HALF_TABLE_HEIGHT);
            GL.TexCoord2(0, table.TextureHeight); GL.Vertex2(PLAY_AREA_CENTRE_X - HALF_TABLE_WIDTH, PLAY_AREA_CENTRE_Y + HALF_TABLE_HEIGHT);
            GL.TexCoord2(table.TextureWidth, table.TextureHeight);
            GL.Vertex2(PLAY_AREA_CENTRE_X + HALF_TABLE_WIDTH, PLAY_AREA_CENTRE_Y + HALF_TABLE_HEIGHT);
            GL.TexCoord2(table.TextureWidth, 0); GL.Vertex2(PLAY_AREA_CENTRE_X + HALF_TABLE_WIDTH, PLAY_AREA_CENTRE_Y - HALF_TABLE_HEIGHT);
            GL.End();

            GL.BindTexture(TextureTarget.Texture2D, OpenGLCheckbox.CheckBoxTexture);
            if (readyCheckBox.Visible) readyCheckBox.Draw();

            if (BID_TOGGLES[0].Visible)
            {
                //font.FaceSize(BID_TOGGLES[0].FontSize, FONT_RESOLUTION);
                font = new System.Drawing.Font(fontFamily, BID_TOGGLES[0].FontSize);

                GL.BindTexture(TextureTarget.Texture2D, OpenGLToggle.BackgroundTextureID);
                foreach (OpenGLToggle toggle in BID_TOGGLES) toggle.DrawBackground();

                GL.BindTexture(TextureTarget.Texture2D, OpenGLToggle.ForegroundTextureID);
                foreach (OpenGLToggle toggle in BID_TOGGLES) toggle.DrawForeground();
            }

            if (bidButton.Visible)
            {
                //font.FaceSize(bidButton.FontSize, FONT_RESOLUTION);
                font = new System.Drawing.Font(fontFamily, bidButton.FontSize);
                GL.BindTexture(TextureTarget.Texture2D, OpenGLButton.TextureID);
                bidButton.Draw();
            }

            if (aboutButton.Visible)
            {
                //font.FaceSize(aboutButton.FontSize, FONT_RESOLUTION);
                font = new System.Drawing.Font(fontFamily, aboutButton.FontSize);
                GL.BindTexture(TextureTarget.Texture2D, OpenGLButton.TextureID);
                aboutButton.Draw();
            }
        }

        private void drawPlayerLabels()
        {
            Player player;
            string emptySeatLabel = ClientMain.me.Status == Player.PlayerStatus.LOOKING_FOR_SEAT ? "Select" : "Empty";

            GL.BindTexture(TextureTarget.Texture2D, OpenGLLabel.GreenLabelTexture);
            for (int i = 0; i < PLAYER_LABELS.Length; i += 2)
            {
                player = ClientMain.me.game.players[i];
                PLAYER_LABELS[i].Text = player != null ? player.name : emptySeatLabel;
                PLAYER_LABELS[i].Draw();
            }

            GL.BindTexture(TextureTarget.Texture2D, OpenGLLabel.BlueLabelTexture);
            for (int i = 1; i < PLAYER_LABELS.Length; i += 2)
            {
                player = ClientMain.me.game.players[i];
                PLAYER_LABELS[i].Text = player != null ? player.name : emptySeatLabel;
                PLAYER_LABELS[i].Draw();
            }

            const float BUFFER = 0.085f;
            const int FONT_SIZE = 14; // TODO: Check font size.
            float x, y;
            for (int i = 0; i < 6; i++)
            {
                if (playerSubText[i] != string.Empty)
                {
                    x = PLAYER_LABELS[i].X;
                    y = PLAYER_LABELS[i].Index <= 2 ? PLAYER_LABELS[i].Y - BUFFER : PLAYER_LABELS[i].Y + BUFFER;
                    //font.FaceSize(FONT_SIZE, FONT_RESOLUTION);
                    font = new System.Drawing.Font(fontFamily, FONT_SIZE);
                    drawText(playerSubText[i], x, y, true);
                }
                if (playerSubSubText[i] != string.Empty)
                {
                    x = PLAYER_LABELS[i].X;
                    y = PLAYER_LABELS[i].Index <= 2 ? PLAYER_LABELS[i].Y + BUFFER : PLAYER_LABELS[i].Y - BUFFER;
                    //font.FaceSize(FONT_SIZE, FONT_RESOLUTION);
                    font = new System.Drawing.Font(fontFamily, FONT_SIZE);
                    drawText(playerSubSubText[i], x, y, true);
                }
                if (centreText != string.Empty)
                {
                    x = PLAY_AREA_CENTRE_X;
                    y = PLAY_AREA_CENTRE_Y;
                    //font.FaceSize(FONT_SIZE, FONT_RESOLUTION);
                    font = new System.Drawing.Font(fontFamily, FONT_SIZE);
                    drawText(centreText, x, y, true);
                }
                if (upperText != string.Empty)
                {
                    x = PLAY_AREA_CENTRE_X;
                    y = WINDOW_MAX_Y - 0.1f;
                    //font.FaceSize(36, FONT_RESOLUTION);
                    font = new System.Drawing.Font(fontFamily, FONT_SIZE);
                    drawText(upperText, x, y, true);
                }
            }
        }

        /// <summary>
        /// Draw text.  IMPORTANT: set font size with font.FaceSize before calling this method.
        /// </summary>
        /// <param name="text">Text to draw.</param>
        /// <param name="x">Left edge of text in screen coordinates.</param>
        /// <param name="y">Bottom edge of text in screen coordinates.</param>
        private void drawText(string text, float x, float y, bool centred)
        {
            //if (centred)
            //{
            //    float halfWidth = font.Advance(text) * FONT_SIZE_MODIFIER / 2;
            //    float halfHeight = (font.Ascender() + font.Descender()) * FONT_SIZE_MODIFIER / 2;
            //    drawText(text, x - halfWidth, x + halfWidth, y - halfHeight, y + halfHeight, true);
            //}
            //else
            //{
                drawText(text, x, x, y, y, false);
            //}
        }

        /// <summary>
        /// Draw text.  IMPORTANT: set font size with font.FaceSize before calling this method.
        /// </summary>
        /// <param name="text">Text to draw.</param>
        /// <param name="x1">Left edge of text in screen coordinates.</param>
        /// <param name="x2">Right edge of text in screen coordinates.</param>
        /// <param name="y1">Bottom edge of text in screen coordinates.</param>
        /// <param name="y2">Top edge of text in screen coordinates.</param>
        /// <param name="centred">If true, text is centred horizontally and vertically.</param>
        private void drawText(string text, float x1, float x2, float y1, float y2, bool centred)
        {
            //if (centred)
            //{
            //    float width = font.Advance(text) * FONT_SIZE_MODIFIER;
            //    float x = (x1 + x2 - width) / 2;
            //    float height = (font.Ascender() + font.Descender()) * FONT_SIZE_MODIFIER;
            //    float y = (y1 + y2 - height) / 2;
            //    if (x < x1) x = x1;
            //    if (y < y1) y = y1;
            //    GL.RasterPos2(x, y);
            //}
            //else
            //{
            //    GL.RasterPos2(x1, y1);
            //}
            //font.Render(text);
            // TODO: Fix parameters.
            
            Rectangle r = new Rectangle(coordsToPixels(x1, y1), sizeToPixels(x2 - x1, y2 - y1));
            textPrinter.Begin();
            textPrinter.Print(text, font, Color.White, r, OpenTK.Graphics.TextPrinterOptions.Default, centred ? OpenTK.Graphics.TextAlignment.Center: OpenTK.Graphics.TextAlignment.Near);
            textPrinter.End();
        }

        private void drawHandCards()
        {
            const float OPACITY = 0.5f;
            Player.PlayerStatus status = ClientMain.me.Status;
            bool highlightingEnabled = status == Player.PlayerStatus.CHOOSING_CARD && hightlightLegalCards.Value;

            GL.BindTexture(TextureTarget.Texture2D, OpenGLCard.TextureID);
            foreach (OpenGLCard card in HAND_CARDS)
            {
                card.Alpha = card.Highlight || !highlightingEnabled ? card.Alpha : OPACITY;

                card.Draw();
            }
        }

        private void drawSideBar()
        {
            GL.BindTexture(TextureTarget.Texture2D, SideBar.SideBarTexture);
            GL.Begin(BeginMode.Quads);
            GL.Color4(1, 1, 1, 1);

            // Preferences
            GL.TexCoord2(SideBar.BODY_X1, SideBar.BODY_Y1); GL.Vertex2(WINDOW_MIN_X + PLAY_AREA_WIDTH, WINDOW_MIN_Y + SideBar.PREFS_BODY_Y1);
            GL.TexCoord2(SideBar.BODY_X1, SideBar.BODY_Y2); GL.Vertex2(WINDOW_MIN_X + PLAY_AREA_WIDTH, WINDOW_MIN_Y + SideBar.PREFS_BODY_Y2);
            GL.TexCoord2(SideBar.BODY_X2, SideBar.BODY_Y2); GL.Vertex2(WINDOW_MAX_X, WINDOW_MIN_Y + SideBar.PREFS_BODY_Y2);
            GL.TexCoord2(SideBar.BODY_X2, SideBar.BODY_Y1); GL.Vertex2(WINDOW_MAX_X, WINDOW_MIN_Y + SideBar.PREFS_BODY_Y1);
            GL.TexCoord2(SideBar.TITLE_X1, SideBar.TITLE_Y1); GL.Vertex2(WINDOW_MIN_X + PLAY_AREA_WIDTH, WINDOW_MIN_Y + SideBar.PREFS_TITLE_Y1);
            GL.TexCoord2(SideBar.TITLE_X1, SideBar.TITLE_Y2); GL.Vertex2(WINDOW_MIN_X + PLAY_AREA_WIDTH, WINDOW_MIN_Y + SideBar.PREFS_TITLE_Y2);
            GL.TexCoord2(SideBar.TITLE_X2, SideBar.TITLE_Y2); GL.Vertex2(WINDOW_MAX_X, WINDOW_MIN_Y + SideBar.PREFS_TITLE_Y2);
            GL.TexCoord2(SideBar.TITLE_X2, SideBar.TITLE_Y1); GL.Vertex2(WINDOW_MAX_X, WINDOW_MIN_Y + SideBar.PREFS_TITLE_Y1);

            // Chat
            GL.TexCoord2(SideBar.BODY_X1, SideBar.BODY_Y1); GL.Vertex2(WINDOW_MIN_X + PLAY_AREA_WIDTH, WINDOW_MIN_Y + SideBar.CHAT_BODY_Y1);
            GL.TexCoord2(SideBar.BODY_X1, SideBar.BODY_Y2); GL.Vertex2(WINDOW_MIN_X + PLAY_AREA_WIDTH, WINDOW_MIN_Y + SideBar.CHAT_BODY_Y2);
            GL.TexCoord2(SideBar.BODY_X2, SideBar.BODY_Y2); GL.Vertex2(WINDOW_MAX_X, WINDOW_MIN_Y + SideBar.CHAT_BODY_Y2);
            GL.TexCoord2(SideBar.BODY_X2, SideBar.BODY_Y1); GL.Vertex2(WINDOW_MAX_X, WINDOW_MIN_Y + SideBar.CHAT_BODY_Y1);
            GL.TexCoord2(SideBar.TITLE_X1, SideBar.TITLE_Y1); GL.Vertex2(WINDOW_MIN_X + PLAY_AREA_WIDTH, WINDOW_MIN_Y + SideBar.CHAT_TITLE_Y1);
            GL.TexCoord2(SideBar.TITLE_X1, SideBar.TITLE_Y2); GL.Vertex2(WINDOW_MIN_X + PLAY_AREA_WIDTH, WINDOW_MIN_Y + SideBar.CHAT_TITLE_Y2);
            GL.TexCoord2(SideBar.TITLE_X2, SideBar.TITLE_Y2); GL.Vertex2(WINDOW_MAX_X, WINDOW_MIN_Y + SideBar.CHAT_TITLE_Y2);
            GL.TexCoord2(SideBar.TITLE_X2, SideBar.TITLE_Y1); GL.Vertex2(WINDOW_MAX_X, WINDOW_MIN_Y + SideBar.CHAT_TITLE_Y1);

            // Tricks
            GL.TexCoord2(SideBar.BODY_X1, SideBar.BODY_Y1); GL.Vertex2(WINDOW_MIN_X + PLAY_AREA_WIDTH, WINDOW_MIN_Y + SideBar.TRICKS_BODY_Y1);
            GL.TexCoord2(SideBar.BODY_X1, SideBar.BODY_Y2); GL.Vertex2(WINDOW_MIN_X + PLAY_AREA_WIDTH, WINDOW_MIN_Y + SideBar.TRICKS_BODY_Y2);
            GL.TexCoord2(SideBar.BODY_X2, SideBar.BODY_Y2); GL.Vertex2(WINDOW_MAX_X, WINDOW_MIN_Y + SideBar.TRICKS_BODY_Y2);
            GL.TexCoord2(SideBar.BODY_X2, SideBar.BODY_Y1); GL.Vertex2(WINDOW_MAX_X, WINDOW_MIN_Y + SideBar.TRICKS_BODY_Y1);
            GL.TexCoord2(SideBar.TITLE_X1, SideBar.TITLE_Y1); GL.Vertex2(WINDOW_MIN_X + PLAY_AREA_WIDTH, WINDOW_MIN_Y + SideBar.TRICKS_TITLE_Y1);
            GL.TexCoord2(SideBar.TITLE_X1, SideBar.TITLE_Y2); GL.Vertex2(WINDOW_MIN_X + PLAY_AREA_WIDTH, WINDOW_MIN_Y + SideBar.TRICKS_TITLE_Y2);
            GL.TexCoord2(SideBar.TITLE_X2, SideBar.TITLE_Y2); GL.Vertex2(WINDOW_MAX_X, WINDOW_MIN_Y + SideBar.TRICKS_TITLE_Y2);
            GL.TexCoord2(SideBar.TITLE_X2, SideBar.TITLE_Y1); GL.Vertex2(WINDOW_MAX_X, WINDOW_MIN_Y + SideBar.TRICKS_TITLE_Y1);

            // Call
            GL.TexCoord2(SideBar.BODY_X1, SideBar.BODY_Y1); GL.Vertex2(WINDOW_MIN_X + PLAY_AREA_WIDTH, WINDOW_MIN_Y + SideBar.CALL_BODY_Y1);
            GL.TexCoord2(SideBar.BODY_X1, SideBar.BODY_Y2); GL.Vertex2(WINDOW_MIN_X + PLAY_AREA_WIDTH, WINDOW_MIN_Y + SideBar.CALL_BODY_Y2);
            GL.TexCoord2(SideBar.BODY_X2, SideBar.BODY_Y2); GL.Vertex2(WINDOW_MAX_X, WINDOW_MIN_Y + SideBar.CALL_BODY_Y2);
            GL.TexCoord2(SideBar.BODY_X2, SideBar.BODY_Y1); GL.Vertex2(WINDOW_MAX_X, WINDOW_MIN_Y + SideBar.CALL_BODY_Y1);
            GL.TexCoord2(SideBar.TITLE_X1, SideBar.TITLE_Y1); GL.Vertex2(WINDOW_MIN_X + PLAY_AREA_WIDTH, WINDOW_MIN_Y + SideBar.CALL_TITLE_Y1);
            GL.TexCoord2(SideBar.TITLE_X1, SideBar.TITLE_Y2); GL.Vertex2(WINDOW_MIN_X + PLAY_AREA_WIDTH, WINDOW_MIN_Y + SideBar.CALL_TITLE_Y2);
            GL.TexCoord2(SideBar.TITLE_X2, SideBar.TITLE_Y2); GL.Vertex2(WINDOW_MAX_X, WINDOW_MIN_Y + SideBar.CALL_TITLE_Y2);
            GL.TexCoord2(SideBar.TITLE_X2, SideBar.TITLE_Y1); GL.Vertex2(WINDOW_MAX_X, WINDOW_MIN_Y + SideBar.CALL_TITLE_Y1);

            // Score
            GL.TexCoord2(SideBar.BODY_X1, SideBar.BODY_Y1); GL.Vertex2(WINDOW_MIN_X + PLAY_AREA_WIDTH, WINDOW_MIN_Y + SideBar.SCORE_BODY_Y1);
            GL.TexCoord2(SideBar.BODY_X1, SideBar.BODY_Y2); GL.Vertex2(WINDOW_MIN_X + PLAY_AREA_WIDTH, WINDOW_MIN_Y + SideBar.SCORE_BODY_Y2);
            GL.TexCoord2(SideBar.BODY_X2, SideBar.BODY_Y2); GL.Vertex2(WINDOW_MAX_X, WINDOW_MIN_Y + SideBar.SCORE_BODY_Y2);
            GL.TexCoord2(SideBar.BODY_X2, SideBar.BODY_Y1); GL.Vertex2(WINDOW_MAX_X, WINDOW_MIN_Y + SideBar.SCORE_BODY_Y1);
            GL.TexCoord2(SideBar.TITLE_X1, SideBar.TITLE_Y1); GL.Vertex2(WINDOW_MIN_X + PLAY_AREA_WIDTH, WINDOW_MIN_Y + SideBar.SCORE_TITLE_Y1);
            GL.TexCoord2(SideBar.TITLE_X1, SideBar.TITLE_Y2); GL.Vertex2(WINDOW_MIN_X + PLAY_AREA_WIDTH, WINDOW_MIN_Y + SideBar.SCORE_TITLE_Y2);
            GL.TexCoord2(SideBar.TITLE_X2, SideBar.TITLE_Y2); GL.Vertex2(WINDOW_MAX_X, WINDOW_MIN_Y + SideBar.SCORE_TITLE_Y2);
            GL.TexCoord2(SideBar.TITLE_X2, SideBar.TITLE_Y1); GL.Vertex2(WINDOW_MAX_X, WINDOW_MIN_Y + SideBar.SCORE_TITLE_Y1);

            // Game Title
            GL.TexCoord2(SideBar.BODY_X1, SideBar.BODY_Y1); GL.Vertex2(WINDOW_MIN_X + PLAY_AREA_WIDTH, WINDOW_MIN_Y + SideBar.NAME_BODY_Y1);
            GL.TexCoord2(SideBar.BODY_X1, SideBar.BODY_Y2); GL.Vertex2(WINDOW_MIN_X + PLAY_AREA_WIDTH, WINDOW_MIN_Y + SideBar.NAME_BODY_Y2);
            GL.TexCoord2(SideBar.BODY_X2, SideBar.BODY_Y2); GL.Vertex2(WINDOW_MAX_X, WINDOW_MIN_Y + SideBar.NAME_BODY_Y2);
            GL.TexCoord2(SideBar.BODY_X2, SideBar.BODY_Y1); GL.Vertex2(WINDOW_MAX_X, WINDOW_MIN_Y + SideBar.NAME_BODY_Y1);
            GL.End();

            // Sidebar text
            //font.FaceSize(36, FONT_RESOLUTION);
            font = new System.Drawing.Font(fontFamily, 10);
            drawText("Preferences", WINDOW_MIN_X + PLAY_AREA_WIDTH + SideBar.INDENT, WINDOW_MAX_X,
                WINDOW_MIN_Y + SideBar.PREFS_TITLE_Y1, WINDOW_MIN_Y + SideBar.PREFS_TITLE_Y2, true);
            drawText("Chat", WINDOW_MIN_X + PLAY_AREA_WIDTH + SideBar.INDENT, WINDOW_MAX_X,
                WINDOW_MIN_Y + SideBar.CHAT_TITLE_Y1, WINDOW_MIN_Y + SideBar.CHAT_TITLE_Y2, true);
            drawText("Tricks", WINDOW_MIN_X + PLAY_AREA_WIDTH + SideBar.INDENT, WINDOW_MAX_X,
                WINDOW_MIN_Y + SideBar.TRICKS_TITLE_Y1, WINDOW_MIN_Y + SideBar.TRICKS_TITLE_Y2, true);
            drawText("Call", WINDOW_MIN_X + PLAY_AREA_WIDTH + SideBar.INDENT, WINDOW_MAX_X,
                WINDOW_MIN_Y + SideBar.CALL_TITLE_Y1, WINDOW_MIN_Y + SideBar.CALL_TITLE_Y2, true);
            drawText("Score", WINDOW_MIN_X + PLAY_AREA_WIDTH + SideBar.INDENT, WINDOW_MAX_X,
                WINDOW_MIN_Y + SideBar.SCORE_TITLE_Y1, WINDOW_MIN_Y + SideBar.SCORE_TITLE_Y2, true);

            // Game Title text
            //font.FaceSize(40, FONT_RESOLUTION);
            font = new System.Drawing.Font(fontFamily, 10);
            drawText("Shoot the Moon", WINDOW_MAX_X - (SIDEBAR_WIDTH - SideBar.INDENT), WINDOW_MAX_X,
                WINDOW_MIN_Y + SideBar.NAME_BODY_Y1, WINDOW_MIN_Y + SideBar.NAME_BODY_Y2, true);

            drawScoreAndTricks();
            drawCall();

            // Game Preferences Content
            drawPreferences();

            // Chat content
            //chatBox.Paint();
        }

        private void drawScoreAndTricks()
        {
            for (int i = 0; i < 2; i++)
            {
                SCORE_LABELS[i].Text = ClientMain.me.game.scores[i].ToString();
                TRICK_LABELS[i].Text = ClientMain.me.game.tricks[i].ToString();
            }

            GL.BindTexture(TextureTarget.Texture2D, OpenGLLabel.GreenLabelTexture);
            SCORE_LABELS[0].Draw();
            TRICK_LABELS[0].Draw();

            GL.BindTexture(TextureTarget.Texture2D, OpenGLLabel.BlueLabelTexture);
            SCORE_LABELS[1].Draw();
            TRICK_LABELS[1].Draw();
        }

        private void drawCall()
        {
            if (!CALL_LABEL.Visible) return;

            Bid bid = ClientMain.me.game.leadingBid;

            if (bid == null || bid.bidder == null) return;

            string caller = bid.bidder.name;
            CALL_LABEL.Text = caller;

            int texture = bid.bidder.position % 2 == 0 ? OpenGLLabel.GreenLabelTexture : OpenGLLabel.BlueLabelTexture;

            GL.BindTexture(TextureTarget.Texture2D, texture);
            CALL_LABEL.Draw();
            drawText(bid.ToDisplayString(), WINDOW_MAX_X - (SIDEBAR_WIDTH - SideBar.INDENT) / 2,
                WINDOW_MIN_Y + (11 * SideBar.CALL_BODY_Y1 + 5 * SideBar.CALL_BODY_Y2) / 16, true);
        }

        private void drawPreferences()
        {
            GL.BindTexture(TextureTarget.Texture2D, OpenGLCheckbox.CheckBoxTexture);
            highlightCardsCheckBox.Draw();
        }

        private void drawPlayedCards()
        {
            if (PLAYED_CARDS == null) return;

            GL.BindTexture(TextureTarget.Texture2D, OpenGLCard.HighlightTexture.TextureID);
            foreach (OpenGLCard card in PLAYED_CARDS) if (card != null && card.Highlight) drawCardGlow(card);

            GL.BindTexture(TextureTarget.Texture2D, OpenGLCard.TextureID);
            foreach (OpenGLCard card in PLAYED_CARDS)
                if (card != null)
                    card.Draw();
        }

        private void drawCardGlow(OpenGLCard card)
        {
            GL.Begin(BeginMode.Quads);
            GL.Color4(1, 1, 1, card.Alpha);
            GL.TexCoord2(OpenGLCard.HighlightTextureX1, OpenGLCard.HighlightTextureY1);
            GL.Vertex2(card.HighlightScreenX1, card.HighlightScreenY1);
            GL.TexCoord2(OpenGLCard.HighlightTextureX1, OpenGLCard.HighlightTextureY2);
            GL.Vertex2(card.HighlightScreenX1, card.HighlightScreenY2);
            GL.TexCoord2(OpenGLCard.HighlightTextureX2, OpenGLCard.HighlightTextureY2);
            GL.Vertex2(card.HighlightScreenX2, card.HighlightScreenY2);
            GL.TexCoord2(OpenGLCard.HighlightTextureX2, OpenGLCard.HighlightTextureY1);
            GL.Vertex2(card.HighlightScreenX2, card.HighlightScreenY1);
            GL.End();
        }

        private void InitializePlayArea()
        {
            float ORIG_THETA, ATTRACTION_POINT, THETA;
            const float SPREAD = 0.7f; // 0 => clustered at top and bottom, 1 => angles evenly distributed.
            float LAYOUT_ASPECT_RATIO = table.AspectRatio * 1.2f;
            float LAYOUT_WIDTH = HALF_TABLE_WIDTH;// *0.85;
            float LAYOUT_HEIGHT = LAYOUT_WIDTH / LAYOUT_ASPECT_RATIO;
            const float LABEL_HEIGHT = 0.17f;

            float PLAYER_LABEL_X, PLAYER_LABEL_Y;

            for (int i = 0; i < PLAYED_CARD_X.Length; i++)
            {
                // Daniel Dresser's card placement algorithm (slightly modified)
                ORIG_THETA = (6 - (i + 0.5f)) / PLAYED_CARD_X.Length * (2 * (float)Math.PI);
                ATTRACTION_POINT = (float)Math.PI * (ORIG_THETA < (float)Math.PI ? 0.5f : 1.5f);
                THETA = ATTRACTION_POINT + (ORIG_THETA - ATTRACTION_POINT) * SPREAD;

                PLAYED_CARD_X[i] = PLAY_AREA_CENTRE_X + (float)Math.Cos(THETA) * LAYOUT_WIDTH;
                PLAYED_CARD_Y[i] = PLAY_AREA_CENTRE_Y + (float)Math.Sin(THETA) * LAYOUT_HEIGHT;

                const int PLAYER_LABEL_FONT_SIZE = 14;

                PLAYER_LABEL_X = PLAYED_CARD_X[i];
                PLAYER_LABEL_Y = PLAYED_CARD_Y[i] + (HAND_CARD_HEIGHT / 2 + LABEL_HEIGHT * 0.55f) * (ORIG_THETA < (float)Math.PI ? 1 : -1);
                PLAYER_LABELS[i] = new OpenGLLabel(PLAYER_LABEL_X, PLAYER_LABEL_Y, LABEL_HEIGHT, "Select", PLAYER_LABEL_FONT_SIZE, this,
                    null, new StandardHandler(nameDownHandler), new StandardHandler(nameUpHandler), new StandardHandler(nameResetHandler));
                PLAYER_LABELS[i].Index = i;

                PLAYER_LABELS[i].Status = OpenGLLabel.LabelStatus.SELECTABLE;
            }

            readyCheckBox = new OpenGLCheckbox(PLAY_AREA_CENTRE_X, PLAY_AREA_CENTRE_Y, 0.08f, "I'm Ready!", 36, readyToPlay, this,
                ReadyCheckBoxClickHandler, null, null, null);
            readyCheckBox.Enabled = false;
            readyCheckBox.Visible = false;

            int TOGGLE_FONT_SIZE = 14;
            float TOGGLE_HEIGHT = 0.1f;
            float TOGGLE_WIDTH = TOGGLE_HEIGHT;
            float TOGGLE_BUFFER = TOGGLE_WIDTH / 6;
            float halfWidth = (TOGGLE_WIDTH * 10 + TOGGLE_BUFFER * 9) / 2;
            float x = PLAY_AREA_CENTRE_X - halfWidth + TOGGLE_WIDTH / 2;
            float y = PLAY_AREA_CENTRE_Y + 0.08f;

            // Trick buttons 1 - 8
            for (int i = 0; i < 8; i++)
            {
                BID_TOGGLES[i] = new OpenGLToggle(x, y, TOGGLE_HEIGHT, (i + 1).ToString(), TOGGLE_FONT_SIZE, this,
                    null, new StandardHandler(bidToggleTricksDownHandler),
                    new StandardHandler(bidToggleTricksUpHandler), new StandardHandler(bidToggleTricksResetHandler));
                BID_TOGGLES[i].Index = i + 1;
                BID_TOGGLES[i].Enabled = false;
                BID_TOGGLES[i].Visible = false;
                x += TOGGLE_WIDTH + TOGGLE_BUFFER;
            }

            // Shoot button
            x = PLAY_AREA_CENTRE_X + halfWidth - TOGGLE_WIDTH - TOGGLE_BUFFER / 2;
            float SHOOT_TOGGLE_WIDTH = TOGGLE_WIDTH * 2 + TOGGLE_BUFFER;
            BID_TOGGLES[8] = new OpenGLToggle(x, y, SHOOT_TOGGLE_WIDTH, TOGGLE_HEIGHT, "Shoot", TOGGLE_FONT_SIZE, this,
                    null, new StandardHandler(bidToggleTricksDownHandler),
                    new StandardHandler(bidToggleTricksUpHandler), new StandardHandler(bidToggleTricksResetHandler));
            BID_TOGGLES[8].Index = 9;
            BID_TOGGLES[8].Enabled = false;
            BID_TOGGLES[8].Visible = false;

            halfWidth = (TOGGLE_WIDTH * 6 + TOGGLE_BUFFER * 5) / 2;
            x = PLAY_AREA_CENTRE_X - halfWidth + TOGGLE_WIDTH / 2;
            y = PLAY_AREA_CENTRE_Y - 0.04f;

            // Trump buttons (x6)
            for (int i = 9; i < 15; i++)
            {
                BID_TOGGLES[i] = new OpenGLToggle(x, y, TOGGLE_HEIGHT, string.Empty, TOGGLE_FONT_SIZE, this,
                    null, new StandardHandler(bidToggleTrumpDownHandler),
                    new StandardHandler(bidToggleTrumpUpHandler), new StandardHandler(bidToggleTrumpResetHandler));
                BID_TOGGLES[i].Icon = Trump.allTrumps[i - 9];
                BID_TOGGLES[i].Index = i - 9;
                BID_TOGGLES[i].Enabled = false;
                BID_TOGGLES[i].Visible = false;
                x += TOGGLE_WIDTH + TOGGLE_BUFFER;
            }

            x = PLAY_AREA_CENTRE_X;
            y = PLAY_AREA_CENTRE_Y - 0.16f;

            // Bid button
            bidButton = new OpenGLButton(x, y, TOGGLE_HEIGHT, "Pass", TOGGLE_FONT_SIZE, this,
                null, new StandardHandler(bidButtonDownHandler),
                new StandardHandler(bidButtonUpHandler), new StandardHandler(bidButtonResetHandler));
            bidButton.Enabled = false;
            bidButton.Visible = false;

            // About button
            x = WINDOW_MIN_X + 0.2f;
            y = WINDOW_MAX_Y - 0.1f;
            aboutButton = new OpenGLButton(x, y, TOGGLE_HEIGHT, "About", TOGGLE_FONT_SIZE, this,
                null, new StandardHandler(aboutButtonDownHandler),
                new StandardHandler(aboutButtonUpHandler), new StandardHandler(aboutButtonResetHandler));
        }

        private void InitializeSidebar()
        {
            const float SPACING_BETWEEN_SCORE_LABELS = 0.03f;
            const float LABEL_HEIGHT = 0.14f;
            const int FONT_SIZE = 32;

            float CENTRE_X, y, TEXT_WIDTH;

            CENTRE_X = WINDOW_MAX_X - (SIDEBAR_WIDTH - SideBar.INDENT) / 2;
            //TEXT_WIDTH = font.Advance("00") * FONT_SIZE_MODIFIER;
            OpenTK.Graphics.TextExtents te = textPrinter.Measure("00", font); // TODO: Fix this.
            //TEXT_WIDTH = te.BoundingBox.Width;
            TEXT_WIDTH = 0.08f;

            y = WINDOW_MIN_Y + (SideBar.SCORE_BODY_Y1 + SideBar.SCORE_BODY_Y2) / 2;
            SCORE_LABELS[0] = new OpenGLLabel(WINDOW_MAX_X - SIDEBAR_WIDTH * 0.75f, y, TEXT_WIDTH, LABEL_HEIGHT, "0", FONT_SIZE, this); // need to create label and get width before we can set the correct X
            SCORE_LABELS[0].X = CENTRE_X - (SPACING_BETWEEN_SCORE_LABELS + SCORE_LABELS[0].ScreenWidth) / 2;

            y = WINDOW_MIN_Y + (SideBar.SCORE_BODY_Y1 + SideBar.SCORE_BODY_Y2) / 2;
            SCORE_LABELS[1] = new OpenGLLabel(WINDOW_MAX_X - SIDEBAR_WIDTH * 0.25f, y, TEXT_WIDTH, LABEL_HEIGHT, "0", FONT_SIZE, this);
            SCORE_LABELS[1].X = CENTRE_X + (SPACING_BETWEEN_SCORE_LABELS + SCORE_LABELS[1].ScreenWidth) / 2;

            y = WINDOW_MIN_Y + (SideBar.TRICKS_BODY_Y1 + SideBar.TRICKS_BODY_Y2) / 2;
            TRICK_LABELS[0] = new OpenGLLabel(WINDOW_MAX_X - SIDEBAR_WIDTH * 0.75f, y, LABEL_HEIGHT, "0", FONT_SIZE, this);
            TRICK_LABELS[0].X = CENTRE_X - (SPACING_BETWEEN_SCORE_LABELS + TRICK_LABELS[0].ScreenWidth) / 2;

            y = WINDOW_MIN_Y + (SideBar.TRICKS_BODY_Y1 + SideBar.TRICKS_BODY_Y2) / 2;
            TRICK_LABELS[1] = new OpenGLLabel(WINDOW_MAX_X - SIDEBAR_WIDTH * 0.25f, y, LABEL_HEIGHT, "0", FONT_SIZE, this);
            TRICK_LABELS[1].X = CENTRE_X + (SPACING_BETWEEN_SCORE_LABELS + TRICK_LABELS[1].ScreenWidth) / 2;

            y = WINDOW_MIN_Y + (5 * SideBar.CALL_BODY_Y1 + 11 * SideBar.CALL_BODY_Y2) / 16;
            CALL_LABEL = new OpenGLLabel(CENTRE_X, y, LABEL_HEIGHT, string.Empty, FONT_SIZE, this);

            highlightCardsCheckBox = new OpenGLCheckbox(WINDOW_MAX_X - SIDEBAR_WIDTH / 2,
                WINDOW_MIN_Y + (SideBar.PREFS_BODY_Y1 + SideBar.PREFS_BODY_Y2 * 3) / 4,
                0.06f, "Highlight legal cards", 28, hightlightLegalCards, this, HighlightCardsCheckBoxClickHandler, null, null, null);
        }

        public void InitializeGame()
        {
            readyToPlay.Value = false;

            rootBB = new BoundingBox(WINDOW_MIN_X, WINDOW_MIN_Y, WINDOW_MAX_X, WINDOW_MAX_Y, null, null, null, null, null);
            HAND_CARDS = new List<OpenGLCard>();
            PLAYED_CARDS = new OpenGLCard[6];
            playerSubText = new string[] { string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty };
            playerSubSubText = new string[] { string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty };
            centreText = string.Empty;
            drawList = new List<OpenGLControl>();
            animationList = new List<OpenGLControl>();
            animationTimer.Change(0, 1000 / FRAMERATE);
            InitializePlayArea();
            InitializeSidebar();
            UpdateChatLog();

            canvas.Invalidate();
        }

        public void endGame()
        {
            //centreText = "GAME OVER";
            readyCheckBox.Checked = false;
            readyCheckBox.Text = "Play Again?";
            readyCheckBox.Visible = true;
            readyCheckBox.Enabled = true;

            TRICK_LABELS[0].Visible = false;
            TRICK_LABELS[1].Visible = false;
            CALL_LABEL.Visible = false;

            canvas.Invalidate();
        }

        public void initializeBiddingPhase()
        {
            readyCheckBox.Enabled = false;
            readyCheckBox.Visible = false;
            foreach (OpenGLToggle toggle in BID_TOGGLES)
            {
                toggle.Visible = true;
            }
            bidButton.Visible = true;

            tentativeTricks = 0;
            tentativeTrump = null;
            activeTrickToggle = null;
            activeTrumpToggle = null;
            bidButton.Text = "Pass";
            playerSubText = new string[] { string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty };
            playerSubSubText = new string[] { string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty };
            centreText = string.Empty;
        }

        private void SetBiddingControlsStatus(bool active)
        {
            Bid winningBid = ClientMain.me.game.leadingBid;
            int tricks = winningBid != null ? winningBid.getNumber() : 0;

            for (int i = 0; i < tricks; i++)
            {
                BID_TOGGLES[i].Enabled = false;
            }
            for (int i = tricks; i < BID_TOGGLES.Length; i++)
            {
                BID_TOGGLES[i].Enabled = active;
            }
            BID_TOGGLES[8].Enabled = active; // shoot button should always be active.
            bidButton.Enabled = active;
        }

        public void EndBiddingPhase()
        {
            playerSubText = new string[] { string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty }; // Stop showing bids above names
            playerSubSubText = new string[] { string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty };
            centreText = string.Empty;
            foreach (OpenGLToggle toggle in BID_TOGGLES)
            {
                toggle.Visible = false;
            }
            bidButton.Visible = false;
        }

        public void initializePlayingPhase()
        {
            readyCheckBox.Enabled = false;
            readyCheckBox.Visible = false;
            for (int i = 0; i < playerSubText.Length; i++)
            {
                playerSubText[i] = string.Empty;
                playerSubSubText[i] = string.Empty;
            }
            centreText = string.Empty;
            UpdateHand();
            UpdateHandHighlighting();
        }

        public void EndPlayingPhase()
        {
        }

        public void initializeTrick()
        {
            //if (PLAYED_CARDS != null)
            //    foreach (OpenGLCard card in PLAYED_CARDS)
            //        if (card != null) card.Visible = false; // clean up bounding boxes from old cards
            PLAYED_CARDS = new OpenGLCard[6];
        }

        public void EndTrick()
        {
            const int TRICK_END_TRAVEL_TIME = 600;

            if (ClientMain.me.game.winningCard != null)
            {
                OpenGLLabel winnerLabel = PLAYER_LABELS[ClientMain.me.game.winningCard.player.position];
                foreach (OpenGLCard card in PLAYED_CARDS)
                {
                    if (card != null)
                    {
                        lock (drawList) drawList.Add(card); // TODO: Add the rest of the controls to drawList and remove from other draw methods?
                        System.Console.WriteLine("Added played cards to draw list");
                        card.Animate(card.X, card.Y, winnerLabel.X, winnerLabel.Y, TRICK_END_TRAVEL_TIME, 1, 0, true);
                    }
                }
            }
        }

        public void assignSeat(Player player)
        {
            PLAYER_LABELS[player.position].Text = player.name;
            if (player.Equals(ClientMain.me))
            {
                lock (PLAYER_LABELS)
                {
                    // rotate human player into bottom middle spot.  TODO: Animation could probably be improved.
                    if (player.position != 1)
                    {
                        int distance = (1 - player.position + PLAYER_LABELS.Length) % PLAYER_LABELS.Length;
                        OpenGLLabel thisLabel, nextLabel;
                        const int ROTATION_TIME = 500;

                        for (int i = 0; i < PLAYER_LABELS.Length; i++)
                        {
                            thisLabel = PLAYER_LABELS[i];
                            nextLabel = PLAYER_LABELS[(i + distance) % PLAYER_LABELS.Length];
                            thisLabel.Index = (i + distance) % PLAYER_LABELS.Length;
                            thisLabel.Animate(thisLabel.X, thisLabel.Y, nextLabel.X, nextLabel.Y, ROTATION_TIME, false);
                        }
                    }
                }

                foreach (OpenGLLabel label in PLAYER_LABELS)
                {
                    if (label.Status != OpenGLLabel.LabelStatus.GLOW) label.Status = OpenGLLabel.LabelStatus.STANDARD;
                    label.Enabled = false;
                }
                if (ClientMain.me.Status == Player.PlayerStatus.PREGAME_NOT_READY || ClientMain.me.Status == Player.PlayerStatus.PREGAME_READY)
                {
                    readyCheckBox.Enabled = true;
                    readyCheckBox.Visible = true;
                }
            }
            else
            {
                PLAYER_LABELS[player.position].Enabled = false;
                if (player.Status == Player.PlayerStatus.CHOOSING_CARD || player.Status == Player.PlayerStatus.CHOOSING_BID)
                    PLAYER_LABELS[player.position].Status = OpenGLLabel.LabelStatus.GLOW;
                else
                    PLAYER_LABELS[player.position].Status = OpenGLLabel.LabelStatus.STANDARD;
            }
            canvas.Invalidate();
        }

        public void emptySeat(int position)
        {
            if (ClientMain.me.Status == Player.PlayerStatus.LOOKING_FOR_SEAT)
            {
                PLAYER_LABELS[position].Enabled = true;
                PLAYER_LABELS[position].Status = OpenGLLabel.LabelStatus.SELECTABLE;
                PLAYER_LABELS[position].Text = "Select";
            }
            else
                PLAYER_LABELS[position].Text = "Empty";

            canvas.Invalidate();
        }

        public void ThrowAwayCard()
        {
            const int DISCARD_TRAVEL_TIME = 250;

            OpenGLCard discard = HAND_CARDS[SELECTED_CARD_INDEX];
            discard.Highlight = false;
            discard.Animate(discard.X, discard.Y, WINDOW_MIN_X, WINDOW_MIN_Y, DISCARD_TRAVEL_TIME, 1, 0, true);
            lock (drawList) drawList.Add(discard);
            System.Console.WriteLine("Added throwaway to draw list");
            HAND_CARDS.RemoveAt(SELECTED_CARD_INDEX);
            for (int i = SELECTED_CARD_INDEX; i < HAND_CARDS.Count; i++) HAND_CARDS[i].Index--;

            canvas.Invalidate();
        }

        public void TransferCard()
        {
            const int DISCARD_TRAVEL_TIME = 250;

            OpenGLCard discard = HAND_CARDS[SELECTED_CARD_INDEX];
            discard.Highlight = false;
            discard.Animate(discard.X, discard.Y, WINDOW_MIN_X, WINDOW_MIN_Y, DISCARD_TRAVEL_TIME, 1, 0, true);
            lock (drawList) drawList.Add(discard);
            HAND_CARDS.RemoveAt(SELECTED_CARD_INDEX);
            for (int i = SELECTED_CARD_INDEX; i < HAND_CARDS.Count; i++) HAND_CARDS[i].Index--;

            canvas.Invalidate();
        }

        public void UpdateHand()
        {
            List<Card> hand = ClientMain.me.hand;
            int handCount = hand.Count;

            float[] destinationX = new float[handCount];
            bool[] deal = new bool[handCount];
            List<OpenGLCard> oldHand = new List<OpenGLCard>(HAND_CARDS);

            Trump currentTrump = ClientMain.me.game.currentTrump;
            if (currentTrump != null) ClientMain.me.sortHand(currentTrump);
            else if (tentativeTrump != null) ClientMain.me.sortHand(tentativeTrump);
            else ClientMain.me.sortHand(Trump.HIGH);

            HAND_CARDS.Clear();

            float CARD_HAND_Y = -0.98f + HAND_CARD_HEIGHT / 2;
            float CARD_BUFFER = HAND_CARD_WIDTH * RELATIVE_HAND_CARD_BUFFER_WIDTH;
            float FIRST_CARD_HAND_X = PLAY_AREA_CENTRE_X - ((handCount / 2f - 0.5f) * HAND_CARD_WIDTH + (handCount / 2f - 0.5f) * CARD_BUFFER);
            float x, y;
            bool foundCard = false;
            OpenGLCard newCard;
            OpenGLCard markedForDeletion = null;

            for (int i = 0; i < handCount; i++)
            {
                x = FIRST_CARD_HAND_X + i * (CARD_BUFFER + HAND_CARD_WIDTH);
                y = CARD_HAND_Y;
                foundCard = false;
                foreach (OpenGLCard card in oldHand)
                {
                    if (card.Card.Equals(hand[i]))
                    {
                        HAND_CARDS.Insert(i, card);
                        markedForDeletion = card;
                        card.Index = i;
                        card.Animate(card.X, card.Y, x, y, 500, false);
                        foundCard = true;
                        break;
                    }
                }
                if (foundCard) oldHand.Remove(markedForDeletion);
                else
                {
                    newCard = new OpenGLCard();
                    newCard.SuspendLayout();
                    newCard.X = x;
                    newCard.Y = y;
                    newCard.Card = hand[i];
                    newCard.Index = i;
                    newCard.ScreenHeight = HAND_CARD_HEIGHT;
                    newCard.ScreenWidth = HAND_CARD_HEIGHT * OpenGLCard.AspectRatio;
                    newCard.ClickHandler = new StandardHandler(cardClickHandler);
                    newCard.Deal();
                    newCard.ResumeLayout();
                    HAND_CARDS.Insert(i, newCard);
                }
                //HAND_CARDS.Add(new OpenGLCard(x, y, HAND_CARD_HEIGHT, hand[i], new StandardHandler(cardClickHandler), null, null, null));
            }

            canvas.Invalidate();
        }

        private void UpdateHandHighlighting()
        {
            List<Card> legalCards;
            Card leadCard = ClientMain.me.game.leadCard;
            Trump currentTrump = ClientMain.me.game.currentTrump;

            legalCards = currentTrump != null ? Rules.getLegalCards(ClientMain.me.hand, leadCard, currentTrump) : new List<Card>();

            if (myStatus == Player.PlayerStatus.CHOOSING_CARD) // && leadCard != null)
                foreach (OpenGLCard card in HAND_CARDS)
                {
                    card.Alpha = 1;
                    card.Highlight = legalCards.Contains(card.Card);
                }

            canvas.Invalidate();
        }

        public void UpdatePlayedCards(Card card)
        {
            const int PLAYED_CARD_TRAVEL_TIME = 350;
            const float INITIAL_OPACITY = 0;

            if (card == null || PLAYED_CARDS == null) return;

            while (PLAYER_LABELS[0].Animating)
                System.Threading.Thread.Sleep(10);

            int position = card.player.position;
            int adjustedPosition = PLAYER_LABELS[position].Index;
            bool me = ClientMain.me.Equals(card.player);
            OpenGLCard playedCard = null;

            if (me && SELECTED_CARD_INDEX > -1)
            {
                playedCard = HAND_CARDS[SELECTED_CARD_INDEX]; // TODO: Fix error when rejoining immediately after replacement AI plays a card
                playedCard.Highlight = false;
                playedCard.Animate(playedCard.X, playedCard.Y, PLAYED_CARD_X[adjustedPosition], PLAYED_CARD_Y[adjustedPosition],
                    PLAYED_CARD_TRAVEL_TIME, INITIAL_OPACITY, 1, false);
                HAND_CARDS.RemoveAt(SELECTED_CARD_INDEX);
                for (int i = SELECTED_CARD_INDEX; i < HAND_CARDS.Count; i++) HAND_CARDS[i].Index--;
            }
            else
            {
                playedCard = new OpenGLCard();
                playedCard.SuspendLayout();
                playedCard.Card = card;
                playedCard.ScreenHeight = HAND_CARD_HEIGHT;
                playedCard.ScreenWidth = HAND_CARD_HEIGHT * OpenGLCard.AspectRatio;
                playedCard.Animate(PLAYER_LABELS[position].X, PLAYER_LABELS[position].Y,
                    PLAYED_CARD_X[adjustedPosition], PLAYED_CARD_Y[adjustedPosition], PLAYED_CARD_TRAVEL_TIME, INITIAL_OPACITY, 1, false);
                playedCard.ResumeLayout();
            }

            PLAYED_CARDS[position] = playedCard;

            //if (playedCard.Card.Equals(ClientMain.me.game.winningCard))
            if (ClientMain.me.game.winningCard != null && playedCard.Card.player.Equals(ClientMain.me.game.winningCard.player))
            {
                if (WINNING_CARD != null) WINNING_CARD.Highlight = false;
                WINNING_CARD = playedCard;
                playedCard.Highlight = true;
            }

            canvas.Invalidate();
        }

        public void UpdateChatLog()
        {
            chatOutputBox.SuspendLayout();
            chatOutputBox.Text = string.Empty;

            foreach (string message in ClientMain.me.game.chatLog)
            {
                chatOutputBox.Text = message + "\n" + chatOutputBox.Text;
            }

            chatOutputBox.ResumeLayout();
        }

        public void UpdateConnectionStatus(Player player)
        {
            Player.PlayerStatus enteringStatus = player.Status;
            Player.PlayerStatus leavingStatus = player.LastStatus;

            //if (enteringStatus == leavingStatus) return; // avoid needless work

            bool me = player.Equals(ClientMain.me);
            if (me) myStatus = enteringStatus;

            switch (leavingStatus)
            {
                case Player.PlayerStatus.CHOOSING_BID:
                    PLAYER_LABELS[player.position].Status = OpenGLLabel.LabelStatus.STANDARD;
                    if (me) SetBiddingControlsStatus(false);
                    break;
                case Player.PlayerStatus.CHOOSING_CARD:
                    PLAYER_LABELS[player.position].Status = OpenGLLabel.LabelStatus.STANDARD;
                    if (me) UpdateHandHighlighting();
                    break;
                case Player.PlayerStatus.CHOOSING_TRANSFER_CARDS:
                    PLAYER_LABELS[player.position].Status = OpenGLLabel.LabelStatus.STANDARD;
                    break;
                case Player.PlayerStatus.THROWING_AWAY_CARDS:
                    PLAYER_LABELS[player.position].Status = OpenGLLabel.LabelStatus.STANDARD;
                    break;
                default:
                    break;
            }

            switch (enteringStatus)
            {
                case Player.PlayerStatus.PREGAME_NOT_READY:
                    playerSubText[player.position] = string.Empty;
                    playerSubSubText[player.position] = string.Empty;
                    centreText = string.Empty;
                    break;
                case Player.PlayerStatus.PREGAME_READY:
                    playerSubText[player.position] = "Ready";
                    break;
                case Player.PlayerStatus.CHOOSING_BID:
                    PLAYER_LABELS[player.position].Status = OpenGLLabel.LabelStatus.GLOW;
                    if (me) SetBiddingControlsStatus(true);
                    break;
                case Player.PlayerStatus.CHOOSING_CARD:
                    PLAYER_LABELS[player.position].Status = OpenGLLabel.LabelStatus.GLOW;
                    if (me) UpdateHandHighlighting();
                    break;
                case Player.PlayerStatus.CHOOSING_TRANSFER_CARDS:
                    PLAYER_LABELS[player.position].Status = OpenGLLabel.LabelStatus.GLOW;
                    break;
                case Player.PlayerStatus.THROWING_AWAY_CARDS:
                    PLAYER_LABELS[player.position].Status = OpenGLLabel.LabelStatus.GLOW;
                    break;
                case Player.PlayerStatus.SITTING_OUT:
                    foreach (OpenGLCard card in HAND_CARDS) card.Visible = false;
                    ClientMain.me.hand.Clear();
                    UpdateHand();
                    break;
                default:
                    break;
            }

            canvas.Invalidate();
        }

        public void UpdateBid(Bid bid)
        {
            playerSubText[bid.bidder.position] = bid.ToDisplayString();
            canvas.Invalidate();
        }

        public void UpdateTricks()
        {
            canvas.Invalidate();
        }

        public void UpdateScore()
        {
            canvas.Invalidate();
        }

        private void UpdateAnimations(Object state)
        {
            if (SHOW_FRAMERATE)
            {
                try { Invoke(new NoParamsHandler(UpdateFramerate)); }
                catch { }
            }

            lock (animationList)
            {
                if (animationList == null || animationList.Count == 0) return;

                List<OpenGLControl> markedForDeletion = new List<OpenGLControl>();

                foreach (OpenGLControl control in animationList) if (control.UpdatePosition()) markedForDeletion.Add(control);

                foreach (OpenGLControl control in markedForDeletion) animationList.Remove(control);
            }
            Invoke(new NoParamsHandler(canvas.Refresh));
        }

        public void UpdateWarningTimer(int position, int secondsLeft)
        {
            string text = secondsLeft >= 0 ? "Timeout in " + secondsLeft : string.Empty;
            if (position > -1)
                playerSubSubText[position] = text;
            else
                centreText = text;
            canvas.Invalidate();
        }

        public void UpdateUpperStatusBar(string message)
        {
            upperText = message;
        }

        private void UpdateFramerate()
        {
            elapsedTime = DateTime.Now - startedTiming;
            currentFramerate = frameCounter / (float)elapsedTime.TotalSeconds;
            if (elapsedTime.TotalMilliseconds >= 500)
            {
                startedTiming = DateTime.Now;
                frameCounter = 0;
                this.Text = "Shoot the Moon: " + currentFramerate.ToString() + " fps";
            }
        }

        #region Event Handlers

        private void FormClosingHandler(object sender, FormClosingEventArgs e)
        {
            if (Visible && !ClientMain.loggingout)
            {
                Player.sendMessageToServer("<LEAVINGGAME>");
                e.Cancel = true;
            }
        }

        private void keyPressHandler(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            //if (selectedControl is OpenGLTextBox)
            //{
            //    selectedControl.Text += e.KeyChar.ToString();
            //}
        }

        private void lostFocusHandler(object sender, EventArgs e)
        {
            mouseIsDown = false;
        }

        private void mouseClickHandler(object sender, MouseEventArgs e)
        {
            float WINDOW_WIDTH = WINDOW_MAX_X - WINDOW_MIN_X;
            float WINDOW_HEIGHT = WINDOW_MAX_Y - WINDOW_MIN_Y;
            float x = WINDOW_MIN_X + WINDOW_WIDTH * (float)e.X / (float)canvas.ClientSize.Width;
            float y = WINDOW_MIN_Y + WINDOW_HEIGHT * (float)(canvas.ClientSize.Height - e.Y) / (float)canvas.ClientSize.Height;

            rootBB.Click(x, y, BoundingBox.ClickType.Click);
        }

        private void mouseDownHandler(object sender, MouseEventArgs e)
        {
            mouseIsDown = true;

            float WINDOW_WIDTH = WINDOW_MAX_X - WINDOW_MIN_X;
            float WINDOW_HEIGHT = WINDOW_MAX_Y - WINDOW_MIN_Y;
            float x = WINDOW_MIN_X + WINDOW_WIDTH * (float)e.X / (float)canvas.ClientSize.Width;
            float y = WINDOW_MIN_Y + WINDOW_HEIGHT * (float)(canvas.ClientSize.Height - e.Y) / (float)canvas.ClientSize.Height;

            rootBB.Click(x, y, BoundingBox.ClickType.Down);
        }

        private void mouseUpHandler(object sender, MouseEventArgs e)
        {
            mouseIsDown = false;

            float WINDOW_WIDTH = WINDOW_MAX_X - WINDOW_MIN_X;
            float WINDOW_HEIGHT = WINDOW_MAX_Y - WINDOW_MIN_Y;
            float x = WINDOW_MIN_X + WINDOW_WIDTH * (float)e.X / (float)canvas.ClientSize.Width;
            float y = WINDOW_MIN_Y + WINDOW_HEIGHT * (float)(canvas.ClientSize.Height - e.Y) / (float)canvas.ClientSize.Height;

            if (depressed != null && depressed.Contains(x, y)) depressed = null;

            rootBB.Click(x, y, BoundingBox.ClickType.Up);
        }

        private void mouseMoveHandler(object sender, MouseEventArgs e)
        {
            if (mouseIsDown && depressed == null)
            {
                float WINDOW_WIDTH = WINDOW_MAX_X - WINDOW_MIN_X;
                float WINDOW_HEIGHT = WINDOW_MAX_Y - WINDOW_MIN_Y;
                float x = WINDOW_MIN_X + WINDOW_WIDTH * (float)e.X / (float)canvas.ClientSize.Width;
                float y = WINDOW_MIN_Y + WINDOW_HEIGHT * (float)(canvas.ClientSize.Height - e.Y) / (float)canvas.ClientSize.Height;

                rootBB.Click(x, y, BoundingBox.ClickType.Down);
            }
            if (depressed != null)
            {
                float WINDOW_WIDTH = WINDOW_MAX_X - WINDOW_MIN_X;
                float WINDOW_HEIGHT = WINDOW_MAX_Y - WINDOW_MIN_Y;
                float x = WINDOW_MIN_X + WINDOW_WIDTH * (float)e.X / (float)canvas.ClientSize.Width;
                float y = WINDOW_MIN_Y + WINDOW_HEIGHT * (float)(canvas.ClientSize.Height - e.Y) / (float)canvas.ClientSize.Height;

                if (!depressed.Contains(x, y))
                {
                    depressed.Reset();
                    depressed = null;
                }
            }
        }

        private void chatKeyPressHandler(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                Player.sendMessageToServer("<CHATMESSAGE>" + chatInputBox.Text);
                chatInputBox.Clear();
            }
        }

        private void cardClickHandler(object[] args)
        {
            float x = (float)args[0];
            float y = (float)args[1];
            OpenGLCard card = (OpenGLCard)args[2];

            if (myStatus == Player.PlayerStatus.THROWING_AWAY_CARDS)
            {
                ClientMain.me.throwAwayCard(card.Card);
            }
            else if (myStatus == Player.PlayerStatus.CHOOSING_TRANSFER_CARDS)
            {
                ClientMain.me.transferCard(card.Card);
            }
            else if (myStatus == Player.PlayerStatus.CHOOSING_CARD)
            {
                ClientMain.me.playCard(card.Card);
            }

            SELECTED_CARD_INDEX = card.Index;
        }

        private void HighlightCardsCheckBoxClickHandler(object[] args)
        {
            OpenGLCheckbox checkBox = args[2] as OpenGLCheckbox;
            checkBox.Checked = !checkBox.Checked;
            UpdateHandHighlighting();
            canvas.Invalidate();
        }

        private void ReadyCheckBoxClickHandler(object[] args)
        {
            OpenGLCheckbox ready = args[2] as OpenGLCheckbox;
            ready.Checked = !ready.Checked;
            Player.sendMessageToServer("<READYSTATUS>" + (ready.Checked ? 1 : 0));
            canvas.Invalidate();
        }

        private void nameClickHandler(object[] args)
        {
        }

        private void nameDownHandler(object[] args)
        {
            OpenGLLabel label = args[2] as OpenGLLabel;
            label.Status = OpenGLLabel.LabelStatus.DEPRESSED;
            canvas.Invalidate();
        }

        private void nameUpHandler(object[] args)
        {
            OpenGLLabel label = args[2] as OpenGLLabel;
            if (label.Status == OpenGLLabel.LabelStatus.DEPRESSED)
            {
                Player.sendMessageToServer("<REQUESTSEAT>" + label.Index.ToString());
                label.Status = OpenGLLabel.LabelStatus.SELECTABLE;
                canvas.Invalidate();
            }
        }

        private void nameResetHandler(object[] args)
        {
            OpenGLLabel label = args[0] as OpenGLLabel;
            label.Status = OpenGLLabel.LabelStatus.SELECTABLE;
            canvas.Invalidate();
        }

        private void bidToggleTricksDownHandler(object[] args)
        {
            OpenGLToggle toggle = args[2] as OpenGLToggle;
            toggle.State = OpenGLToggle.ToggleState.DEPRESSED;
            canvas.Invalidate();
        }

        private void bidToggleTricksUpHandler(object[] args)
        {
            OpenGLToggle toggle = args[2] as OpenGLToggle;
            if (activeTrickToggle != null && toggle.Index == activeTrickToggle.Index)
            {
                toggle.State = OpenGLToggle.ToggleState.STANDARD;
                activeTrickToggle = null;
                tentativeTricks = 0;
            }
            else
            {
                toggle.State = OpenGLToggle.ToggleState.SELECTED;
                if (activeTrickToggle != null) activeTrickToggle.State = OpenGLToggle.ToggleState.STANDARD;
                activeTrickToggle = toggle;
                tentativeTricks = toggle.Index;
            }
            bidButton.Text = activeTrickToggle != null && activeTrumpToggle != null ? "Bid" : "Pass";
            canvas.Invalidate();
        }

        private void bidToggleTricksResetHandler(object[] args)
        {
            OpenGLToggle toggle = args[0] as OpenGLToggle;
            if (activeTrickToggle != null && toggle.Index == activeTrickToggle.Index) toggle.State = OpenGLToggle.ToggleState.SELECTED;
            else toggle.State = OpenGLToggle.ToggleState.STANDARD;
            canvas.Invalidate();
        }

        private void bidToggleTrumpDownHandler(object[] args)
        {
            OpenGLToggle toggle = args[2] as OpenGLToggle;
            toggle.State = OpenGLToggle.ToggleState.DEPRESSED;
            canvas.Invalidate();
        }

        private void bidToggleTrumpUpHandler(object[] args)
        {
            OpenGLToggle toggle = args[2] as OpenGLToggle;
            if (activeTrumpToggle != null && toggle.Index == activeTrumpToggle.Index)
            {
                toggle.State = OpenGLToggle.ToggleState.STANDARD;
                activeTrumpToggle = null;
                tentativeTrump = null;
            }
            else
            {
                toggle.State = OpenGLToggle.ToggleState.SELECTED;
                if (activeTrumpToggle != null) activeTrumpToggle.State = OpenGLToggle.ToggleState.STANDARD;
                activeTrumpToggle = toggle;
                tentativeTrump = Trump.allTrumps[toggle.Index];
            }
            bidButton.Text = activeTrickToggle != null && activeTrumpToggle != null ? "Bid" : "Pass";

            UpdateHand();

            canvas.Invalidate();
        }

        private void bidToggleTrumpResetHandler(object[] args)
        {
            OpenGLToggle toggle = args[0] as OpenGLToggle;
            if (activeTrumpToggle != null && toggle.Index == activeTrumpToggle.Index) toggle.State = OpenGLToggle.ToggleState.SELECTED;
            else toggle.State = OpenGLToggle.ToggleState.STANDARD;
            canvas.Invalidate();
        }

        private void bidButtonDownHandler(object[] args)
        {
            OpenGLButton button = args[2] as OpenGLButton;
            button.State = OpenGLButton.ButtonState.DEPRESSED;
            canvas.Invalidate();
        }

        private void bidButtonUpHandler(object[] args)
        {
            OpenGLButton button = args[2] as OpenGLButton;
            button.State = OpenGLButton.ButtonState.STANDARD;
            canvas.Invalidate();

            string trump = tentativeTrump == null ? string.Empty : tentativeTrump.ToString(); ;
            int shootNum = tentativeTricks == 9 ? ClientMain.me.game.nextShootNum : 0;
            Player.sendMessageToServer("<BID>" + tentativeTricks + shootNum + trump);
        }

        private void bidButtonResetHandler(object[] args)
        {
            OpenGLButton button = args[0] as OpenGLButton;
            button.State = OpenGLButton.ButtonState.STANDARD;
            canvas.Invalidate();
        }

        private void aboutButtonDownHandler(object[] args)
        {
            OpenGLButton button = args[2] as OpenGLButton;
            button.State = OpenGLButton.ButtonState.DEPRESSED;
            canvas.Invalidate();
        }

        private void aboutButtonUpHandler(object[] args)
        {
            OpenGLButton button = args[2] as OpenGLButton;
            button.State = OpenGLButton.ButtonState.STANDARD;
            canvas.Invalidate();

            ClientMain.aboutScreen.Show();
        }

        private void aboutButtonResetHandler(object[] args)
        {
            OpenGLButton button = args[0] as OpenGLButton;
            button.State = OpenGLButton.ButtonState.STANDARD;
            canvas.Invalidate();
        }

        #endregion

        #region Helper Classes

        private static class SideBar
        {
            private static Texture texture;
            public static int SideBarTexture
            {
                get { return texture.TextureID; }
            }

            #region coordinate data

            private const float SIDEBAR_WIDTH_PIXELS = 256;
            private const float SIDEBAR_HEIGHT_PIXELS = 768;
            public const float SIDEBAR_ASPECT_RATIO = SIDEBAR_WIDTH_PIXELS / SIDEBAR_HEIGHT_PIXELS;

            private const float TEXTURE_WIDTH_PIXELS = 256;
            private const float TEXTURE_HEIGHT_PIXELS = 256;

            private const float BODY_HEIGHT_PIXELS = 102;
            private const float BODY_ASPECT_RATIO = SIDEBAR_WIDTH_PIXELS / BODY_HEIGHT_PIXELS;
            private const float BODY_X1_PIXELS = 0;
            public const float BODY_X1 = BODY_X1_PIXELS / TEXTURE_WIDTH_PIXELS;
            private const float BODY_X2_PIXELS = SIDEBAR_WIDTH_PIXELS;
            public const float BODY_X2 = BODY_X2_PIXELS / TEXTURE_WIDTH_PIXELS;
            private const float BODY_Y1_PIXELS = 0;
            public const float BODY_Y1 = BODY_Y1_PIXELS / TEXTURE_HEIGHT_PIXELS;
            private const float BODY_Y2_PIXELS = BODY_HEIGHT_PIXELS;
            public const float BODY_Y2 = BODY_Y2_PIXELS / TEXTURE_HEIGHT_PIXELS;

            private const float TITLE_HEIGHT_PIXELS = 26;
            private const float TITLE_ASPECT_RATIO = SIDEBAR_WIDTH_PIXELS / TITLE_HEIGHT_PIXELS;
            private const float TITLE_X1_PIXELS = 0;
            public const float TITLE_X1 = TITLE_X1_PIXELS / TEXTURE_WIDTH_PIXELS;
            private const float TITLE_X2_PIXELS = SIDEBAR_WIDTH_PIXELS;
            public const float TITLE_X2 = TITLE_X2_PIXELS / TEXTURE_WIDTH_PIXELS;
            private const float TITLE_Y1_PIXELS = BODY_HEIGHT_PIXELS;
            public const float TITLE_Y1 = TITLE_Y1_PIXELS / TEXTURE_HEIGHT_PIXELS;
            private const float TITLE_Y2_PIXELS = TITLE_Y1_PIXELS + TITLE_HEIGHT_PIXELS;
            public const float TITLE_Y2 = TITLE_Y2_PIXELS / TEXTURE_HEIGHT_PIXELS;

            private const float PREFS_BODY_Y_PIXELS = 0;
            public static float PREFS_BODY_Y1 = PREFS_BODY_Y_PIXELS / SIDEBAR_HEIGHT_PIXELS * SIDEBAR_HEIGHT;
            private const float PREFS_BODY_HEIGHT_PIXELS = 86;
            public static float PREFS_BODY_Y2 = (PREFS_BODY_Y_PIXELS + PREFS_BODY_HEIGHT_PIXELS) / SIDEBAR_HEIGHT_PIXELS * SIDEBAR_HEIGHT;
            private const float PREFS_TITLE_Y_PIXELS = PREFS_BODY_Y_PIXELS + PREFS_BODY_HEIGHT_PIXELS;
            public static float PREFS_TITLE_Y1 = PREFS_TITLE_Y_PIXELS / SIDEBAR_HEIGHT_PIXELS * SIDEBAR_HEIGHT;
            public static float PREFS_TITLE_Y2 = (PREFS_TITLE_Y_PIXELS + TITLE_HEIGHT_PIXELS) / SIDEBAR_HEIGHT_PIXELS * SIDEBAR_HEIGHT;

            private const float CHAT_BODY_Y_PIXELS = PREFS_TITLE_Y_PIXELS + TITLE_HEIGHT_PIXELS;
            public static float CHAT_BODY_Y1 = CHAT_BODY_Y_PIXELS / SIDEBAR_HEIGHT_PIXELS * SIDEBAR_HEIGHT;
            private const float CHAT_BODY_HEIGHT_PIXELS = 265;
            public static float CHAT_BODY_Y2 = (CHAT_BODY_Y_PIXELS + CHAT_BODY_HEIGHT_PIXELS) / SIDEBAR_HEIGHT_PIXELS * SIDEBAR_HEIGHT;
            private const float CHAT_TITLE_Y_PIXELS = CHAT_BODY_Y_PIXELS + CHAT_BODY_HEIGHT_PIXELS;
            public static float CHAT_TITLE_Y1 = CHAT_TITLE_Y_PIXELS / SIDEBAR_HEIGHT_PIXELS * SIDEBAR_HEIGHT;
            public static float CHAT_TITLE_Y2 = (CHAT_TITLE_Y_PIXELS + TITLE_HEIGHT_PIXELS) / SIDEBAR_HEIGHT_PIXELS * SIDEBAR_HEIGHT;

            private const float TRICKS_BODY_Y_PIXELS = CHAT_TITLE_Y_PIXELS + TITLE_HEIGHT_PIXELS;
            public static float TRICKS_BODY_Y1 = TRICKS_BODY_Y_PIXELS / SIDEBAR_HEIGHT_PIXELS * SIDEBAR_HEIGHT;
            private const float TRICKS_BODY_HEIGHT_PIXELS = 64;
            public static float TRICKS_BODY_Y2 = (TRICKS_BODY_Y_PIXELS + TRICKS_BODY_HEIGHT_PIXELS) / SIDEBAR_HEIGHT_PIXELS * SIDEBAR_HEIGHT;
            private const float TRICKS_TITLE_Y_PIXELS = TRICKS_BODY_Y_PIXELS + TRICKS_BODY_HEIGHT_PIXELS;
            public static float TRICKS_TITLE_Y1 = TRICKS_TITLE_Y_PIXELS / SIDEBAR_HEIGHT_PIXELS * SIDEBAR_HEIGHT;
            public static float TRICKS_TITLE_Y2 = (TRICKS_TITLE_Y_PIXELS + TITLE_HEIGHT_PIXELS) / SIDEBAR_HEIGHT_PIXELS * SIDEBAR_HEIGHT;

            private const float CALL_BODY_Y_PIXELS = TRICKS_TITLE_Y_PIXELS + TITLE_HEIGHT_PIXELS;
            public static float CALL_BODY_Y1 = CALL_BODY_Y_PIXELS / SIDEBAR_HEIGHT_PIXELS * SIDEBAR_HEIGHT;
            private const float CALL_BODY_HEIGHT_PIXELS = 97;
            public static float CALL_BODY_Y2 = (CALL_BODY_Y_PIXELS + CALL_BODY_HEIGHT_PIXELS) / SIDEBAR_HEIGHT_PIXELS * SIDEBAR_HEIGHT;
            private const float CALL_TITLE_Y_PIXELS = CALL_BODY_Y_PIXELS + CALL_BODY_HEIGHT_PIXELS;
            public static float CALL_TITLE_Y1 = CALL_TITLE_Y_PIXELS / SIDEBAR_HEIGHT_PIXELS * SIDEBAR_HEIGHT;
            public static float CALL_TITLE_Y2 = (CALL_TITLE_Y_PIXELS + TITLE_HEIGHT_PIXELS) / SIDEBAR_HEIGHT_PIXELS * SIDEBAR_HEIGHT;

            private const float SCORE_BODY_Y_PIXELS = CALL_TITLE_Y_PIXELS + TITLE_HEIGHT_PIXELS;
            public static float SCORE_BODY_Y1 = SCORE_BODY_Y_PIXELS / SIDEBAR_HEIGHT_PIXELS * SIDEBAR_HEIGHT;
            private const float SCORE_BODY_HEIGHT_PIXELS = 62;
            public static float SCORE_BODY_Y2 = (SCORE_BODY_Y_PIXELS + SCORE_BODY_HEIGHT_PIXELS) / SIDEBAR_HEIGHT_PIXELS * SIDEBAR_HEIGHT;
            private const float SCORE_TITLE_Y_PIXELS = SCORE_BODY_Y_PIXELS + SCORE_BODY_HEIGHT_PIXELS;
            public static float SCORE_TITLE_Y1 = SCORE_TITLE_Y_PIXELS / SIDEBAR_HEIGHT_PIXELS * SIDEBAR_HEIGHT;
            public static float SCORE_TITLE_Y2 = (SCORE_TITLE_Y_PIXELS + TITLE_HEIGHT_PIXELS) / SIDEBAR_HEIGHT_PIXELS * SIDEBAR_HEIGHT;

            private const float NAME_BODY_Y_PIXELS = SCORE_TITLE_Y_PIXELS + TITLE_HEIGHT_PIXELS;
            public static float NAME_BODY_Y1 = NAME_BODY_Y_PIXELS / SIDEBAR_HEIGHT_PIXELS * SIDEBAR_HEIGHT;
            private const float NAME_BODY_HEIGHT_PIXELS = 64;
            public static float NAME_BODY_Y2 = (NAME_BODY_Y_PIXELS + NAME_BODY_HEIGHT_PIXELS) / SIDEBAR_HEIGHT_PIXELS * SIDEBAR_HEIGHT;

            private const float INDENT_WIDTH_PIXELS = 21;
            public static float INDENT = 21 / SIDEBAR_WIDTH_PIXELS * SIDEBAR_WIDTH;

            #endregion

            static SideBar()
            {
                texture = Texture.LoadTexture("Sidebar.png", TEXTURE_WIDTH_PIXELS, TEXTURE_HEIGHT_PIXELS);
            }

            /// <summary>
            /// Delete any stored texture data.  IMPORTANT: Textures belonging to this class will become unusable.
            /// </summary>
            public static void DeleteTextures()
            {
                texture.DeleteTextureData();
            }
        }

        protected abstract class OpenGLControl
        {
            const float FRAME_LENGTH = 1000f / FRAMERATE;

            private OpenGLTableScreen parent = null;
            protected OpenGLTableScreen Parent
            {
                get { return parent; }
                set { parent = value; }
            }

            private bool layoutSuspended = false;
            protected bool LayoutSuspended
            {
                get { return layoutSuspended; }
                set { layoutSuspended = value; }
            }

            protected float x = 0;
            /// <summary>
            /// The x-value of the control's centre in screen coordinates.
            /// </summary>
            public float X
            {
                get { return x; }
                set
                {
                    x = value;
                    if (!LayoutSuspended) RecalculateBounds();
                }
            }

            protected float y = 0;
            /// <summary>
            /// The y-value of the control's centre in screen coordinates.
            /// </summary>
            public float Y
            {
                get { return y; }
                set
                {
                    y = value;
                    if (!LayoutSuspended) RecalculateBounds();
                }
            }

            protected float toX = 0;
            protected float ToX
            {
                get { return toX; }
                set { toX = value; }
            }

            protected float toY = 0;
            protected float ToY
            {
                get { return toY; }
                set { toY = value; }
            }

            private float velocityX = 0;
            protected float VelocityX
            {
                get { return velocityX; }
            }

            private float velocityY = 0;
            protected float VelocityY
            {
                get { return velocityY; }
            }

            private float alpha = 1;
            public float Alpha
            {
                get { return alpha; }
                set { alpha = value; }
            }

            private float toAlpha = 1;
            protected float ToAlpha
            {
                get { return toAlpha; }
                set { toAlpha = value; }
            }

            private float alphaVelocity = 0;
            protected float AlphaVelocity
            {
                get { return alphaVelocity; }
                set { alphaVelocity = value; }
            }

            protected float screenWidth = 0;
            /// <summary>
            /// Width of the control in screen coordinates.
            /// </summary>
            public float ScreenWidth
            {
                get { return screenWidth; }
                set
                {
                    screenWidth = value;
                    if (!LayoutSuspended) RecalculateBounds();
                }
            }

            protected float screenHeight = 0;
            /// <summary>
            /// Height of the control in screen coordinates.
            /// </summary>
            public float ScreenHeight
            {
                get { return screenHeight; }
                set
                {
                    screenHeight = value;
                    if (!LayoutSuspended) RecalculateBounds();
                }
            }

            /// <summary>
            /// A Bounding Box with the same screen coordinates as this control.
            /// </summary>
            protected BoundingBox bb = null;

            protected bool animating = false;
            /// <summary>
            /// While Animating is true, the control has no Bounding Box.  Setting to false restores the Bounding Box.
            /// </summary>
            public bool Animating
            {
                get { return animating; }
                set
                {
                    animating = value;
                    if (!LayoutSuspended)
                    {
                        if (animating && bb != null)
                        {
                            bb.RemoveFromParent();
                            bb = null;
                        }
                        else if (!animating && bb == null) CreateBoundingBox();
                    }
                }
            }

            protected float animationDistance = 0;
            protected float AnimationDistance
            {
                get { return animationDistance; }
                set { animationDistance = value; }
            }

            protected float animationProgress = 0;
            protected float AnimationProgress
            {
                get { return animationProgress; }
                set
                {
                    if (value < 0 || value > 100) throw new Exception("Animation progress must be between 0 and 100");
                    animationProgress = value;
                }
            }

            protected bool hideAfterAnimation = false;
            /// <summary>
            /// If true, deletes the control after animation is finished.  Otherwise, the control remains and a Bounding Box is formed.
            /// </summary>
            protected bool HideAfterAnimation
            {
                get { return hideAfterAnimation; }
                set { hideAfterAnimation = value; }
            }

            protected string text = string.Empty;
            /// <summary>
            /// The text to be displayed on the control.
            /// </summary>
            public virtual string Text
            {
                get { return text; }
                set { text = value; }
            }

            protected int fontSize = 0;
            /// <summary>
            /// The size of the text to be displayed on the control.
            /// </summary>
            public int FontSize
            {
                get { return fontSize; }
                set { fontSize = value; }
            }

            protected bool enabled = true;
            /// <summary>
            /// Determines whether this control can be clicked on.
            /// </summary>
            public virtual bool Enabled
            {
                get { return enabled; }
                set
                {
                    enabled = value;
                    if (bb != null) bb.Enabled = enabled;
                }
            }

            protected bool visible = true;
            /// <summary>
            /// Determines whether this control is visible.
            /// </summary>
            public bool Visible
            {
                get { return visible; }
                set
                {
                    visible = value;
                    if (!LayoutSuspended)
                    {
                        if (visible && bb == null) CreateBoundingBox();
                        else if (!visible && bb != null)
                        {
                            bb.RemoveFromParent();
                            bb = null;
                        }
                    }
                }
            }

            /// <summary>
            /// Handles mouse clicks.
            /// </summary>
            protected StandardHandler clickHandler = null;
            public StandardHandler ClickHandler
            {
                get { return clickHandler; }
                set { clickHandler = value; }
            }
            /// <summary>
            /// Handles mouse-down events.
            /// </summary>
            protected StandardHandler mouseDownHandler = null;
            public StandardHandler MouseDownHandler
            {
                get { return mouseDownHandler; }
                set { mouseDownHandler = value; }
            }
            /// <summary>
            /// Handles mouse-up events.
            /// </summary>
            protected StandardHandler mouseUpHandler = null;
            public StandardHandler MouseUpHandler
            {
                get { return mouseUpHandler; }
                set { mouseUpHandler = value; }
            }
            /// <summary>
            /// Handles focus loss when the mouse button is down.
            /// </summary>
            protected StandardHandler resetHandler = null;
            public StandardHandler ResetHandler
            {
                get { return resetHandler; }
                set { resetHandler = value; }
            }

            protected int index = -1;
            /// <summary>
            /// The index of this control within an array (optional).
            /// </summary>
            public int Index
            {
                get { return index; }
                set { index = value; }
            }

            protected float textureX1 = 0;
            protected virtual float TextureX1
            {
                get { return textureX1; }
                set { textureX1 = value; }
            }
            protected float textureX2 = 0;
            protected virtual float TextureX2
            {
                get { return textureX2; }
                set { textureX2 = value; }
            }
            protected float textureY1 = 0;
            protected virtual float TextureY1
            {
                get { return textureY1; }
                set { textureY1 = value; }
            }
            protected float textureY2 = 0;
            protected virtual float TextureY2
            {
                get { return textureY2; }
                set { textureY2 = value; }
            }

            protected float screenX1 = 0;
            protected float ScreenX1
            {
                get { return screenX1; }
                set { screenX1 = value; }
            }
            protected float screenX2 = 0;
            protected float ScreenX2
            {
                get { return screenX2; }
                set { screenX2 = value; }
            }
            protected float screenY1 = 0;
            protected float ScreenY1
            {
                get { return screenY1; }
                set { screenY1 = value; }
            }
            protected float screenY2 = 0;
            protected float ScreenY2
            {
                get { return screenY2; }
                set { screenY2 = value; }
            }

            public OpenGLControl()
            {
            }

            public OpenGLControl(float x, float y, float width, float height, string text, int fontSize, OpenGLTableScreen parent,
                StandardHandler mouseClick, StandardHandler mouseDown, StandardHandler mouseUp, StandardHandler reset)
            {
                SuspendLayout();

                Parent = parent;

                X = x;
                Y = y;
                FontSize = fontSize;
                Text = text;
                ScreenWidth = width;
                ScreenHeight = height;

                clickHandler = mouseClick;
                mouseDownHandler = mouseDown;
                mouseUpHandler = mouseUp;
                resetHandler = reset;

                ResumeLayout();

                //RecalculateBounds();
            }

            public OpenGLControl(float x, float y, float width, float height, string text, int fontSize, OpenGLTableScreen parent)
                : this(x, y, width, height, text, fontSize, parent, null, null, null, null)
            {
            }

            /// <summary>
            /// Dummy method to make sure class gets loaded
            /// </summary>
            public static void Initialize()
            {
            }

            public void SuspendLayout()
            {
                LayoutSuspended = true;
            }

            public void ResumeLayout()
            {
                LayoutSuspended = false;

                RecalculateBounds();
                CreateBoundingBox();
            }

            /// <summary>
            /// Create a Bounding Box with the same coordinates as this control.
            /// </summary>
            private void CreateBoundingBox()
            {
                if (!Visible || Animating || bb != null) return;

                bb = new BoundingBox(screenX1, ScreenY1, ScreenX2, ScreenY2, clickHandler, mouseDownHandler, mouseUpHandler, resetHandler, this);
                rootBB.AddChildBox(bb);
                bb.Enabled = Enabled;
            }

            /// <summary>
            /// Update graphic and Bounding Box coordinates.
            /// </summary>
            protected virtual void RecalculateBounds()
            {
                float halfWidth = ScreenWidth / 2;
                ScreenX1 = x - halfWidth;
                ScreenX2 = x + halfWidth;

                float halfHeight = ScreenHeight / 2;
                ScreenY1 = y - halfHeight;
                ScreenY2 = y + halfHeight;

                if (bb != null) bb.Move(ScreenX1, ScreenY1, ScreenX2, ScreenY2);
            }

            /// <summary>
            /// Draw a Label.  IMPORTANT: Bind appropriate texture first.
            /// </summary>
            public virtual void Draw()
            {
                if (!Visible) return;

                GL.Begin(BeginMode.Quads);

                GL.Color4(1, 1, 1, Alpha);
                GL.TexCoord2(TextureX1, TextureY1);
                GL.Vertex2(ScreenX1, ScreenY1);
                GL.TexCoord2(TextureX1, TextureY2);
                GL.Vertex2(ScreenX1, ScreenY2);
                GL.TexCoord2(TextureX2, TextureY2);
                GL.Vertex2(ScreenX2, ScreenY2);
                GL.TexCoord2(TextureX2, TextureY1);
                GL.Vertex2(ScreenX2, ScreenY1);

                GL.End();

                if (Text != string.Empty)
                {
                    //font.FaceSize(FontSize, FONT_RESOLUTION);
                    font = new System.Drawing.Font(fontFamily, FontSize);
                    Parent.drawText(Text, x, y, true);
                }
            }

            /// <summary>
            /// Move a control from one point to another on the screen.
            /// </summary>
            /// <param name="fromX">Original x coordinate.</param>
            /// <param name="fromY">Original y coordinate.</param>
            /// <param name="toX">New x coordinate.</param>
            /// <param name="toY">New y coordinate.</param>
            /// <param name="time">Time elapsed during animation in milliseconds.</param>
            /// <param name="hideAfterAnimation">If true, control is hidden after animation is complete.</param>
            public void Animate(float fromX, float fromY, float toX, float toY, int time, bool hideAfterAnimation)
            {
                X = fromX;
                Y = fromY;
                ToX = toX;
                ToY = toY;
                AnimationDistance = (float)Math.Sqrt(Math.Pow(toX - fromX, 2) + Math.Pow(toY - fromY, 2));
                velocityX = (toX - fromX) / (float)time * FRAME_LENGTH;
                velocityY = (toY - fromY) / (float)time * FRAME_LENGTH;
                Animating = true;
                HideAfterAnimation = hideAfterAnimation;
                lock (animationList) animationList.Add(this);
            }

            public void Animate(float fromX, float fromY, float toX, float toY, int time,
                float fromAlpha, float toAlpha, bool hideAfterAnimation)
            {
                if (fromAlpha < 0 || fromAlpha > 1 || toAlpha < 0 || toAlpha > 1) throw new Exception("Alpha must be between 0 and 1.");

                Alpha = fromAlpha;
                ToAlpha = toAlpha;
                AlphaVelocity = (toAlpha - fromAlpha) / (float)time * FRAME_LENGTH;
                Animate(fromX, fromY, toX, toY, time, hideAfterAnimation);
            }

            /// <summary>
            /// Update a control's animation
            /// </summary>
            /// <returns>true if object has reached its destination; false otherwise</returns>
            public bool UpdatePosition()
            {
                const float tolerance = 0.01f; // rounding errors can make the control overshoot its target.
                float gapX = Math.Abs(ToX - X);
                float gapY = Math.Abs(ToY - Y);

                //if ((gapX <= Math.Abs(VelocityX) * tolerance && gapY <= gapX) || (gapY <= Math.Abs(VelocityY) * tolerance && gapX <= gapY))
                if ((gapX <= tolerance && gapY <= gapX) || (gapY <= tolerance && gapX <= gapY))
                {
                    X = ToX;
                    Y = ToY;
                    Alpha = ToAlpha;
                    AnimationProgress = 100;
                    if (HideAfterAnimation) Visible = false;
                    Animating = false;
                    return true;
                }

                X += VelocityX;
                Y += VelocityY;
                Alpha += AlphaVelocity;
                AnimationProgress = 100 * (float)Math.Round((1 - Math.Sqrt(Math.Pow(gapX, 2) + Math.Pow(gapY, 2)) / AnimationDistance), 2);
                return false;
            }
        }

        private class OpenGLCheckbox : OpenGLControl
        {
            private static Texture checkBoxTexture;
            /// <summary>
            /// Checkbox texture ID.
            /// </summary>
            public static int CheckBoxTexture
            {
                get { return checkBoxTexture.TextureID; }
            }

            private Bool flag;
            private Bool Flag
            {
                get { return flag; }
                set { flag = value; }
            }

            public bool Checked
            {
                get { return Flag.Value; }
                set
                {
                    Flag.Value = value;
                    currentOffsetX = Flag.Value ? CHECKED_TEXTURE_OFFSET_X : UNCHECKED_TEXTURE_OFFSET_X;
                    currentOffsetY = Flag.Value ? CHECKED_TEXTURE_OFFSET_Y : UNCHECKED_TEXTURE_OFFSET_Y;
                    TextureX1 = TEXTURE_X1_BASE + currentOffsetX;
                    TextureX2 = TEXTURE_X2_BASE + currentOffsetX;
                    TextureY1 = TEXTURE_Y1_BASE + currentOffsetY;
                    TextureY2 = TEXTURE_Y2_BASE + currentOffsetY;
                }
            }

            private static float TEXTURE_X1_BASE;
            private static float TEXTURE_X2_BASE;
            private static float TEXTURE_Y1_BASE;
            private static float TEXTURE_Y2_BASE;
            private const float CHECKED_TEXTURE_OFFSET_X = 0;
            private const float CHECKED_TEXTURE_OFFSET_Y = 0;
            private const float UNCHECKED_TEXTURE_OFFSET_X = 0.5f;
            private const float UNCHECKED_TEXTURE_OFFSET_Y = 0;
            private float currentOffsetX = 0;
            private float currentOffsetY = 0;

            private float ImageScreenX1;
            private float ImageScreenX2;
            private float ImageScreenY1;
            private float ImageScreenY2;

            static OpenGLCheckbox()
            {
                checkBoxTexture = Texture.LoadTexture("checkbox.png", 36, 44); // 71, 88); // 14, 18);
                TEXTURE_X1_BASE = 0 ;
                TEXTURE_X2_BASE = checkBoxTexture .PixelWidth / checkBoxTexture .CanvasWidth ;
                TEXTURE_Y1_BASE = 0;
                TEXTURE_Y2_BASE = checkBoxTexture.PixelHeight / checkBoxTexture.CanvasHeight;
            }

            public OpenGLCheckbox(float x, float y, float height, string text, int fontSize, Bool flag, OpenGLTableScreen parent,
                StandardHandler mouseClick, StandardHandler mouseDown, StandardHandler mouseUp, StandardHandler reset)
                : base(x, y, 0, height, text, fontSize, parent, mouseClick, mouseDown, mouseUp, reset) {
                Flag = flag ;
                Checked = flag .Value ;
                //font .FaceSize(fontSize, FONT_RESOLUTION);
                font = new System.Drawing.Font(fontFamily, FontSize);
                ScreenWidth = ScreenHeight * checkBoxTexture.AspectRatio /*+ font.Advance(" " + text) * FONT_SIZE_MODIFIER*/ ; // Fix this.
            }

            public OpenGLCheckbox(float x, float y, float height, string text, int fontSize, Bool flag, OpenGLTableScreen parent)
                : this(x, y, height, text, fontSize, flag, parent, null, null, null, null)
            {
            }

            /// <summary>
            /// Draw a Checkbox.  IMPORTANT: Bind appropriate texture first.
            /// </summary>
            public override void Draw()
            {
                if (!Visible) return;

                GL.Begin(BeginMode.Quads);

                // draw checkbox
                GL.TexCoord2(TextureX1, TextureY1);
                GL.Vertex2(ImageScreenX1, ImageScreenY1);
                GL.TexCoord2(TextureX1, TextureY2);
                GL.Vertex2(ImageScreenX1, ImageScreenY2);
                GL.TexCoord2(TextureX2, TextureY2);
                GL.Vertex2(ImageScreenX2, ImageScreenY2);
                GL.TexCoord2(TextureX2, TextureY1);
                GL.Vertex2(ImageScreenX2, ImageScreenY1);

                GL.End();

                // draw text
                const float TEXT_LOFT = 0.1f; // how high text is drawn in the control, as a percentage.
                //font.FaceSize(FontSize, FONT_RESOLUTION);
                font = new System.Drawing.Font(fontFamily, FontSize);
                Parent.drawText(" " + Text, ImageScreenX2, ImageScreenY1 + ScreenHeight * TEXT_LOFT, false);
            }

            protected override void RecalculateBounds()
            {
                base.RecalculateBounds();
                ImageScreenX1 = ScreenX1;
                ImageScreenX2 = ImageScreenX1 + ScreenHeight * checkBoxTexture.AspectRatio;
                ImageScreenY1 = ScreenY1;
                ImageScreenY2 = ImageScreenY1 + ScreenHeight;
            }

            /// <summary>
            /// Delete any stored texture data.  IMPORTANT: Textures belonging to this class will become unusable.
            /// </summary>
            public static void DeleteTextures()
            {
                checkBoxTexture.DeleteTextureData();
            }
        }

        private class OpenGLLabel : OpenGLControl
        {
            private static Texture blueTexture;
            /// <summary>
            /// OpenGL Texture ID for a blue label.
            /// </summary>
            public static int BlueLabelTexture
            {
                get { return blueTexture.TextureID; }
            }

            private static Texture greenTexture;
            /// <summary>
            /// OpenGL Texture ID for a green label.
            /// </summary>
            public static int GreenLabelTexture
            {
                get { return greenTexture.TextureID; }
            }

            /// <summary>
            /// Possible graphical states for a label.
            /// </summary>
            public enum LabelStatus { STANDARD, GLOW, SELECTABLE, DEPRESSED };

            private LabelStatus status;
            /// <summary>
            /// The current graphical state of this label.
            /// </summary>
            public LabelStatus Status
            {
                get { return status; }
                set
                {
                    status = value;
                    switch (status)
                    {
                        case LabelStatus.STANDARD:
                            currentOffsetX = STANDARD_TEXTURE_OFFSET_X;
                            currentOffsetY = STANDARD_TEXTURE_OFFSET_Y;
                            break;
                        case LabelStatus.GLOW:
                            currentOffsetX = GLOW_TEXTURE_OFFSET_X;
                            currentOffsetY = GLOW_TEXTURE_OFFSET_Y;
                            break;
                        case LabelStatus.SELECTABLE:
                            currentOffsetX = SELECTABLE_TEXTURE_OFFSET_X;
                            currentOffsetY = SELECTABLE_TEXTURE_OFFSET_Y;
                            break;
                        case LabelStatus.DEPRESSED:
                            currentOffsetX = DEPRESSED_TEXTURE_OFFSET_X;
                            currentOffsetY = DEPRESSED_TEXTURE_OFFSET_Y;
                            break;
                        default:
                            currentOffsetX = STANDARD_TEXTURE_OFFSET_X;
                            currentOffsetY = STANDARD_TEXTURE_OFFSET_Y;
                            break;
                    }

                    left_piece_texture_x1 = LEFT_PIECE_TEXTURE_X_BASE + currentOffsetX;
                    left_piece_texture_x2 = left_piece_texture_x1 + end_piece_texture_width;
                    left_piece_texture_y1 = LEFT_PIECE_TEXTURE_Y_BASE + currentOffsetY;
                    left_piece_texture_y2 = left_piece_texture_y1 + texture_height;

                    float scale_factor = CentreWidth / ScreenHeight / CENTRE_PIECE_ASPECT_RATIO;
                    scale_factor = scale_factor < 1 ? scale_factor : 1;
                    centre_piece_texture_x1 = CENTRE_PIECE_TEXTURE_X_BASE + currentOffsetX;
                    centre_piece_texture_x2 = centre_piece_texture_x1 + scale_factor * CentrePieceTextureWidth;
                    centre_piece_texture_y1 = CENTRE_PIECE_TEXTURE_Y_BASE + currentOffsetY;
                    centre_piece_texture_y2 = centre_piece_texture_y1 + texture_height;

                    right_piece_texture_x1 = RIGHT_PIECE_TEXTURE_X_BASE + currentOffsetX;
                    right_piece_texture_x2 = right_piece_texture_x1 + end_piece_texture_width;
                    right_piece_texture_y1 = RIGHT_PIECE_TEXTURE_Y_BASE + currentOffsetY;
                    right_piece_texture_y2 = right_piece_texture_y1 + texture_height;
                }
            }

            private bool autosize;
            /// <summary>
            /// Determines whether this label should set its size automatically based on the contained text.
            /// </summary>
            public bool AutoSize
            {
                get { return autosize; }
                set
                {
                    autosize = value;
                    if (autosize)
                    {
                        //font.FaceSize(FontSize, FONT_RESOLUTION);
                        font = new System.Drawing.Font(fontFamily, FontSize);
                        //CentreWidth = font.Advance(text) * FONT_SIZE_MODIFIER;
                        OpenTK.Graphics.TextExtents te = textPrinter.Measure(text, font);
                        CentreWidth = Parent.sizetoCoords(te.BoundingBox.Size).Width;
                    }
                }
            }

            /// <summary>
            /// Width of the centre section of the label in screen coordinates.
            /// </summary>
            public float CentreWidth
            {
                get { return ScreenWidth - EndPieceScreenWidth * 2; }
                set
                {
                    ScreenWidth = value + EndPieceScreenWidth * 2;
                    RecalculateBounds();
                }
            }

            /// <summary>
            /// The text to be displayed on the label.
            /// </summary>
            public override string Text
            {
                get { return base.Text; }
                set
                {
                    base.Text = value;

                    //font.FaceSize(FontSize, FONT_RESOLUTION);
                    font = new System.Drawing.Font(fontFamily, FontSize);
                    //SCREEN_HEIGHT = (font.Ascender() + font.Descender()) * FONT_SIZE_MODIFIER * 3.0;
                    if (autosize) AutoSize = true; // recalculate
                }
            }

            #region coordinate data

            // Each label texture file contains all four designs.  These offsets point to the correct image within the file.
            private const float STANDARD_TEXTURE_OFFSET_X = 0;
            private const float STANDARD_TEXTURE_OFFSET_Y = 0;
            private const float GLOW_TEXTURE_OFFSET_X = 0;
            private const float GLOW_TEXTURE_OFFSET_Y = 0.5f;
            private const float SELECTABLE_TEXTURE_OFFSET_X = 0.5f;
            private const float SELECTABLE_TEXTURE_OFFSET_Y = 0;
            private const float DEPRESSED_TEXTURE_OFFSET_X = 0.5f;
            private const float DEPRESSED_TEXTURE_OFFSET_Y = 0.5f;
            private float currentOffsetX = STANDARD_TEXTURE_OFFSET_X;
            private float currentOffsetY = STANDARD_TEXTURE_OFFSET_Y;

            private const float END_PIECE_PIXEL_WIDTH = 32.5f;
            private const float PIXEL_HEIGHT = 67;
            private const float END_PIECE_ASPECT_RATIO = END_PIECE_PIXEL_WIDTH / PIXEL_HEIGHT;
            private float EndPieceScreenWidth;
            private static float end_piece_texture_width;
            private static float EndPieceTextureWidth
            {
                get { return end_piece_texture_width; }
            }
            private static float texture_height;
            private static float TextureHeight
            {
                get { return texture_height; }
            }

            private const float LEFT_PIECE_PIXEL_X = 0;
            private const float LEFT_PIECE_PIXEL_Y = 0;
            private static float LEFT_PIECE_TEXTURE_X_BASE;
            private static float LEFT_PIECE_TEXTURE_Y_BASE;
            private float left_piece_texture_x1;
            private float LeftPieceTextureX1
            {
                get { return left_piece_texture_x1; }
            }
            private float left_piece_texture_y1;
            private float LeftPieceTextureY1
            {
                get { return left_piece_texture_y1; }
            }
            private float left_piece_texture_x2;
            private float LeftPieceTextureX2
            {
                get { return left_piece_texture_x2; }
            }
            private float left_piece_texture_y2;
            private float LeftPieceTextureY2
            {
                get { return left_piece_texture_y2; }
            }
            private float left_piece_screen_x1;
            private float LeftPieceScreenX1
            {
                get { return left_piece_screen_x1; }
            }
            private float left_piece_screen_x2;
            private float LeftPieceScreenX2
            {
                get { return left_piece_screen_x2; }
            }
            private float left_piece_screen_y1;
            private float LeftPieceScreenY1
            {
                get { return left_piece_screen_y1; }
            }
            private float left_piece_screen_y2;
            private float LeftPieceScreenY2
            {
                get { return left_piece_screen_y2; }
            }

            private const float CENTRE_PIECE_PIXEL_X = LEFT_PIECE_PIXEL_X + END_PIECE_PIXEL_WIDTH; // 14;
            private const float CENTRE_PIECE_PIXEL_Y = 0;
            private const float CENTRE_PIECE_PIXEL_WIDTH = 80;
            private const float CENTRE_PIECE_ASPECT_RATIO = CENTRE_PIECE_PIXEL_WIDTH / PIXEL_HEIGHT;
            private static float CENTRE_PIECE_TEXTURE_X_BASE;
            private static float CENTRE_PIECE_TEXTURE_Y_BASE;
            private static float CENTRE_PIECE_TEXTURE_WIDTH_BASE;
            private float centre_piece_texture_x1;
            private float CentrePieceTextureX1
            {
                get { return centre_piece_texture_x1; }
            }
            private float centre_piece_texture_y1;
            private float CentrePieceTextureY1
            {
                get { return centre_piece_texture_y1; }
            }
            private float centre_piece_texture_x2;
            private float CentrePieceTextureX2
            {
                get { return centre_piece_texture_x2; }
            }
            private float centre_piece_texture_y2;
            private float CentrePieceTextureY2
            {
                get { return centre_piece_texture_y2; }
            }
            private static float centre_piece_texture_width;
            private static float CentrePieceTextureWidth
            {
                get { return centre_piece_texture_width; }
            }
            private float centre_piece_screen_x1;
            private float CentrePieceScreenX1
            {
                get { return centre_piece_screen_x1; }
            }
            private float centre_piece_screen_x2;
            private float CentrePieceScreenX2
            {
                get { return centre_piece_screen_x2; }
            }
            private float centre_piece_screen_y1;
            private float CentrePieceScreenY1
            {
                get { return centre_piece_screen_y1; }
            }
            private float centre_piece_screen_y2;
            private float CentrePieceScreenY2
            {
                get { return centre_piece_screen_y2; }
            }

            private const float RIGHT_PIECE_PIXEL_X = CENTRE_PIECE_PIXEL_X + CENTRE_PIECE_PIXEL_WIDTH;
            private const float RIGHT_PIECE_PIXEL_Y = 0;
            private static float RIGHT_PIECE_TEXTURE_X_BASE;
            private static float RIGHT_PIECE_TEXTURE_Y_BASE;
            private float right_piece_texture_x1;
            private float RightPieceTextureX1
            {
                get { return right_piece_texture_x1; }
            }
            private float right_piece_texture_y1;
            private float RightPieceTextureY1
            {
                get { return right_piece_texture_y1; }
            }
            private float right_piece_texture_x2;
            private float RightPieceTextureX2
            {
                get { return right_piece_texture_x2; }
            }
            private float right_piece_texture_y2;
            private float RightPieceTextureY2
            {
                get { return right_piece_texture_y2; }
            }
            private float right_piece_screen_x1;
            private float RightPieceScreenX1
            {
                get { return right_piece_screen_x1; }
            }
            private float right_piece_screen_x2;
            private float RightPieceScreenX2
            {
                get { return right_piece_screen_x2; }
            }
            private float right_piece_screen_y1;
            private float RightPieceScreenY1
            {
                get { return right_piece_screen_y1; }
            }
            private float right_piece_screen_y2;
            private float RightPieceScreenY2
            {
                get { return right_piece_screen_y2; }
            }

            #endregion

            #region constructors

            static OpenGLLabel()
            {
                blueTexture = Texture.LoadTexture("BlueLabel.png", 145, 67); // 50, 28);
                greenTexture = Texture.LoadTexture("GreenLabel.png", 145, 67); // 50, 28);

                end_piece_texture_width = END_PIECE_PIXEL_WIDTH / blueTexture.CanvasWidth;
                texture_height = PIXEL_HEIGHT / blueTexture.CanvasHeight;

                LEFT_PIECE_TEXTURE_X_BASE = LEFT_PIECE_PIXEL_X / blueTexture.CanvasWidth;
                LEFT_PIECE_TEXTURE_Y_BASE = LEFT_PIECE_PIXEL_Y / blueTexture.CanvasHeight;

                CENTRE_PIECE_TEXTURE_X_BASE = CENTRE_PIECE_PIXEL_X / blueTexture.CanvasWidth;
                CENTRE_PIECE_TEXTURE_Y_BASE = CENTRE_PIECE_PIXEL_Y / blueTexture .CanvasHeight ;
                CENTRE_PIECE_TEXTURE_WIDTH_BASE = CENTRE_PIECE_PIXEL_WIDTH / blueTexture .CanvasWidth ;

                RIGHT_PIECE_TEXTURE_X_BASE = RIGHT_PIECE_PIXEL_X / blueTexture.CanvasWidth;
                RIGHT_PIECE_TEXTURE_Y_BASE = RIGHT_PIECE_PIXEL_Y / blueTexture.CanvasHeight;
            }

            public OpenGLLabel(float x, float y, float width, float height, string text, int fontSize, OpenGLTableScreen parent,
                StandardHandler mouseClick, StandardHandler mouseDown, StandardHandler mouseUp, StandardHandler reset)
                :
                base(x, y, width, height, text, fontSize, parent, mouseClick , mouseDown , mouseUp , reset )
            {
                AutoSize = false;
                Status = LabelStatus.STANDARD;

                centre_piece_texture_width = CENTRE_PIECE_TEXTURE_WIDTH_BASE;
                EndPieceScreenWidth = ScreenHeight * END_PIECE_ASPECT_RATIO;
                CentreWidth = width;
            }

            public OpenGLLabel(float x, float y, float height, string text, int fontSize, OpenGLTableScreen parent,
                StandardHandler mouseClick , StandardHandler mouseDown, StandardHandler mouseUp , StandardHandler reset)
                : this (x, y, 0, height, text, fontSize, parent, mouseClick, mouseDown, mouseUp, reset) // can set width to 0 because AutoSize will fix it.
            {
                AutoSize = true;
            }

            public OpenGLLabel( float x , float y, float width , float height, string text, int fontSize, OpenGLTableScreen parent)
                : this(x, y, width, height, text, fontSize, parent, null, null, null, null)
            {
            }

            public OpenGLLabel(float x, float y, float height, string text, int fontSize, OpenGLTableScreen parent)
                : this(x, y, 0, height, text, fontSize, parent, null, null, null, null) // can set width to 0 because AutoSize will fix it.
            {
                AutoSize = true;
            }

            #endregion

            protected override void RecalculateBounds()
            {
                base.RecalculateBounds();

                float halfWidth = ScreenWidth / 2;
                left_piece_screen_x1 = x - halfWidth;
                left_piece_screen_x2 = left_piece_screen_x1 + EndPieceScreenWidth;
                right_piece_screen_x2 = x + halfWidth;
                right_piece_screen_x1 = right_piece_screen_x2 - EndPieceScreenWidth;
                centre_piece_screen_x1 = left_piece_screen_x2;
                centre_piece_screen_x2 = right_piece_screen_x1;

                float halfHeight = ScreenHeight / 2;
                left_piece_screen_y1 = y - halfHeight;
                left_piece_screen_y2 = y + halfHeight;
                right_piece_screen_y1 = left_piece_screen_y1;
                right_piece_screen_y2 = left_piece_screen_y2;
                centre_piece_screen_y1 = left_piece_screen_y1;
                centre_piece_screen_y2 = left_piece_screen_y2;
            }

            /// <summary>
            /// Draw a Label.  IMPORTANT: Bind appropriate texture first.
            /// </summary>
            public override void Draw()
            {
                if (!Visible) return;

                //font.FaceSize(FontSize, FONT_RESOLUTION);
                font = new System.Drawing.Font(fontFamily, FontSize);

                GL.Begin(BeginMode.Quads);

                // left end of player label
                GL.TexCoord2(LeftPieceTextureX1, LeftPieceTextureY1);
                GL.Vertex2(LeftPieceScreenX1, LeftPieceScreenY1);
                GL.TexCoord2(LeftPieceTextureX1, LeftPieceTextureY2);
                GL.Vertex2(LeftPieceScreenX1, LeftPieceScreenY2);
                GL.TexCoord2(LeftPieceTextureX2, LeftPieceTextureY2);
                GL.Vertex2(LeftPieceScreenX2, LeftPieceScreenY2);
                GL.TexCoord2(LeftPieceTextureX2, LeftPieceTextureY1);
                GL.Vertex2(LeftPieceScreenX2, LeftPieceScreenY1);

                // middle of player label
                GL.TexCoord2(CentrePieceTextureX1, CentrePieceTextureY1);
                GL.Vertex2(CentrePieceScreenX1, CentrePieceScreenY1);
                GL.TexCoord2(CentrePieceTextureX1, CentrePieceTextureY2);
                GL.Vertex2(CentrePieceScreenX1, CentrePieceScreenY2);
                GL.TexCoord2(CentrePieceTextureX2, CentrePieceTextureY2);
                GL.Vertex2(CentrePieceScreenX2, CentrePieceScreenY2);
                GL.TexCoord2(CentrePieceTextureX2, CentrePieceTextureY1);
                GL.Vertex2(CentrePieceScreenX2, CentrePieceScreenY1);

                // right end of player label
                GL.TexCoord2(RightPieceTextureX1, RightPieceTextureY1);
                GL.Vertex2(RightPieceScreenX1, RightPieceScreenY1);
                GL.TexCoord2(RightPieceTextureX1, RightPieceTextureY2);
                GL.Vertex2(RightPieceScreenX1, RightPieceScreenY2);
                GL.TexCoord2(RightPieceTextureX2, RightPieceTextureY2);
                GL.Vertex2(RightPieceScreenX2, RightPieceScreenY2);
                GL.TexCoord2(RightPieceTextureX2, RightPieceTextureY1);
                GL.Vertex2(RightPieceScreenX2, RightPieceScreenY1);

                GL.End();

                const float TEXT_LOFT = 0.69f; // how high text is drawn in the control, as a percentage.

                Parent.drawText(Text, LeftPieceScreenX1, RightPieceScreenX2,
                    RightPieceScreenY1 + ScreenHeight * TEXT_LOFT, RightPieceScreenY2 + ScreenHeight * TEXT_LOFT, true);
            }

            /// <summary>
            /// Delete any stored texture data.  IMPORTANT: Textures belonging to this class will become unusable.
            /// </summary>
            public static void DeleteTextures()
            {
                blueTexture.DeleteTextureData();
                greenTexture.DeleteTextureData();
            }
        }

        private class OpenGLButton : OpenGLControl
        {
            public enum ButtonState { STANDARD, DEPRESSED };

            private static Texture texture;
            public static int TextureID
            {
                get { return texture.TextureID; }
            }

            private ButtonState state;
            public ButtonState State
            {
                get { return state; }
                set
                {
                    state = value;
                    switch (state)
                    {
                        case ButtonState.STANDARD:
                        default:
                            TextureX1 = STANDARD_TEXTURE_OFFSET_X;
                            TextureY1 = STANDARD_TEXTURE_OFFSET_Y;
                            break;

                        case ButtonState.DEPRESSED:
                            TextureX1 = DEPRESSED_TEXTURE_OFFSET_X;
                            TextureY1 = DEPRESSED_TEXTURE_OFFSET_Y;
                            break;
                    }

                    TextureX2 = TextureX1 + texture.TextureWidth;
                    TextureY2 = TextureY1 + texture.TextureHeight;
                }
            }

            public Label label;

            // The button texture file contains both designs.  These offsets point to the correct image within the file.
            private const float STANDARD_TEXTURE_OFFSET_X = 0;
            private const float STANDARD_TEXTURE_OFFSET_Y = 0;
            private const float DEPRESSED_TEXTURE_OFFSET_X = 0;
            private const float DEPRESSED_TEXTURE_OFFSET_Y = 0.5f ;

            static OpenGLButton( )
            {
                texture = Texture.LoadTexture("RoundButton.png", 436, 145);
            }

            public OpenGLButton(float x, float y, float width, float height, string text, int fontSize, OpenGLTableScreen parent,
                StandardHandler mouseClick, StandardHandler mouseDown, StandardHandler mouseUp, StandardHandler reset)
                : base(x , y , width , height , text , fontSize, parent , mouseClick , mouseDown, mouseUp, reset)
            {
                State = ButtonState.STANDARD;
            }

            public OpenGLButton(float x, float y, float height, string text, int fontSize, OpenGLTableScreen parent,
                StandardHandler mouseClick, StandardHandler mouseDown, StandardHandler mouseUp, StandardHandler reset)
                : this(x, y, height * texture.AspectRatio, height, text, fontSize, parent, mouseClick, mouseDown, mouseUp, reset)
            {
            }

            public override void Draw()
            {
                // Crude middle-stretching algorithm
                float TEXTURE_MIDPOINT_A = TextureX1 + texture.TextureWidth * 0.3f;
                float TEXTURE_MIDPOINT_B = TextureX2 - texture.TextureWidth * 0.3f;
                float SCREEN_MIDPOINT_A = ScreenX1 + ScreenWidth * 0.3f;
                float SCREEN_MIDPOINT_B = ScreenX2 - ScreenWidth * 0.3f;

                GL.Begin(BeginMode.Quads);

                GL.TexCoord2(TextureX1, TextureY1);
                GL.Vertex2(ScreenX1, ScreenY1);
                GL.TexCoord2(TextureX1, TextureY2);
                GL.Vertex2(ScreenX1, ScreenY2);
                GL.TexCoord2(TEXTURE_MIDPOINT_A, TextureY2);
                GL.Vertex2(SCREEN_MIDPOINT_A, ScreenY2);
                GL.TexCoord2(TEXTURE_MIDPOINT_A, TextureY1);
                GL.Vertex2(SCREEN_MIDPOINT_A, ScreenY1);

                GL.TexCoord2(TEXTURE_MIDPOINT_A, TextureY1);
                GL.Vertex2(SCREEN_MIDPOINT_A, ScreenY1);
                GL.TexCoord2(TEXTURE_MIDPOINT_A, TextureY2);
                GL.Vertex2(SCREEN_MIDPOINT_A, ScreenY2);
                GL.TexCoord2(TEXTURE_MIDPOINT_B, TextureY2);
                GL.Vertex2(SCREEN_MIDPOINT_B, ScreenY2);
                GL.TexCoord2(TEXTURE_MIDPOINT_B, TextureY1);
                GL.Vertex2(SCREEN_MIDPOINT_B, ScreenY1);

                GL.TexCoord2(TEXTURE_MIDPOINT_B, TextureY1);
                GL.Vertex2(SCREEN_MIDPOINT_B, ScreenY1);
                GL.TexCoord2(TEXTURE_MIDPOINT_B, TextureY2);
                GL.Vertex2(SCREEN_MIDPOINT_B, ScreenY2);
                GL.TexCoord2(TextureX2, TextureY2);
                GL.Vertex2(ScreenX2, ScreenY2);
                GL.TexCoord2(TextureX2, TextureY1);
                GL.Vertex2(ScreenX2, ScreenY1);

                GL.End();


                const float TEXT_LOFT = 0.74f; // how high text is drawn in the control, as a percentage.

                Parent.drawText(Text, ScreenX1, ScreenX2,
                    ScreenY1 + ScreenHeight * TEXT_LOFT, ScreenY2 + ScreenHeight * TEXT_LOFT, true);

                //font.FaceSize(FontSize, FONT_RESOLUTION);
                //font = new System.Drawing.Font(fontFamily, FontSize);
                //Parent.drawText(Text, x, y, true);
            }

            /// <summary>
            /// Delete any stored texture data.  IMPORTANT: Textures belonging to this class will become unusable.
            /// </summary>
            public static void DeleteTextures()
            {
                texture.DeleteTextureData();
            }
        }

        private class OpenGLToggle : OpenGLControl
        {
            public enum ToggleState { STANDARD, DISABLED, SELECTED, DEPRESSED };

            private static Texture toggleTexture;
            public static int BackgroundTextureID
            {
                get { return toggleTexture.TextureID; }
            }

            private static Texture suitTexture;
            public static int ForegroundTextureID
            {
                get { return suitTexture.TextureID; }
            }

            private ToggleState state;
            public ToggleState State
            {
                get { return state; }
                set
                {
                    state = value;
                    switch (state)
                    {
                        case ToggleState.STANDARD:
                        default:
                            TextureX1 = STANDARD_TEXTURE_OFFSET_X;
                            TextureY1 = STANDARD_TEXTURE_OFFSET_Y;
                            break;

                        case ToggleState.SELECTED:
                            TextureX1 = SELECTED_TEXTURE_OFFSET_X;
                            TextureY1 = SELECTED_TEXTURE_OFFSET_Y;
                            break;

                        case ToggleState.DISABLED:
                            TextureX1 = DISABLED_TEXTURE_OFFSET_X;
                            TextureY1 = DISABLED_TEXTURE_OFFSET_Y;
                            break;

                        case ToggleState.DEPRESSED:
                            TextureX1 = DEPRESSED_TEXTURE_OFFSET_X;
                            TextureY1 = DEPRESSED_TEXTURE_OFFSET_Y;
                            break;
                    }

                    TextureX2 = TextureX1 + toggleTexture.TextureWidth;
                    TextureY2 = TextureY1 + toggleTexture.TextureHeight;
                }
            }

            private Trump icon;
            public Trump Icon
            {
                get { return icon; }
                set
                {
                    icon = value;
                    text = string.Empty;

                    switch (icon.ToString())
                    {
                        case "Clubs":
                        default:
                            ForeTextureX1 = CLUBS_TEXTURE_OFFSET_X;
                            ForeTextureY1 = CLUBS_TEXTURE_OFFSET_Y;
                            break;
                        case "Spades":
                            ForeTextureX1 = SPADES_TEXTURE_OFFSET_X;
                            ForeTextureY1 = SPADES_TEXTURE_OFFSET_Y;
                            break;
                        case "Hearts":
                            ForeTextureX1 = HEARTS_TEXTURE_OFFSET_X;
                            ForeTextureY1 = HEARTS_TEXTURE_OFFSET_Y;
                            break;
                        case "Diamonds":
                            ForeTextureX1 = DIAMONDS_TEXTURE_OFFSET_X;
                            ForeTextureY1 = DIAMONDS_TEXTURE_OFFSET_Y;
                            break;
                        case "High":
                            ForeTextureX1 = HIGH_TEXTURE_OFFSET_X;
                            ForeTextureY1 = HIGH_TEXTURE_OFFSET_Y;
                            break;
                        case "Low":
                            ForeTextureX1 = LOW_TEXTURE_OFFSET_X;
                            ForeTextureY1 = LOW_TEXTURE_OFFSET_Y;
                            break;
                    }

                    ForeTextureX2 = ForeTextureX1 + suitTexture.TextureWidth;
                    ForeTextureY2 = ForeTextureY1 + suitTexture.TextureHeight;
                }
            }

            public override string Text
            {
                get
                {
                    return base.Text;
                }
                set
                {
                    base.Text = value;
                    icon = null;
                }
            }

            public override bool Enabled
            {
                get
                {
                    return base.Enabled;
                }
                set
                {
                    base.Enabled = value;
                    State = enabled ? ToggleState.STANDARD : ToggleState.DISABLED;
                }
            }

            #region coordinate data

            const float FOREGROUND_RELATIVE_SIZE = 0.6f;

            private float foreScreenWidth;
            private float ForeScreenWidth
            {
                get { return foreScreenWidth; }
                set { foreScreenWidth = value; }
            }
            private float foreScreenHeight;
            private float ForeScreenHeight
            {
                get { return foreScreenHeight; }
                set { foreScreenHeight = value; }
            }

            // The toggle texture file contains all four designs.  These offsets point to the correct image within the file.
            private const float STANDARD_TEXTURE_OFFSET_X = 0;
            private const float STANDARD_TEXTURE_OFFSET_Y = 0;
            private const float DISABLED_TEXTURE_OFFSET_X = 0;
            private const float DISABLED_TEXTURE_OFFSET_Y = 0.5f;
            private const float SELECTED_TEXTURE_OFFSET_X = 0.5f;
            private const float SELECTED_TEXTURE_OFFSET_Y = 0;
            private const float DEPRESSED_TEXTURE_OFFSET_X = 0.5f;
            private const float DEPRESSED_TEXTURE_OFFSET_Y = 0.5f;

            // The suit texture file contains all six designs.  These offsets point to the correct image within the file.
            private const float CLUBS_TEXTURE_OFFSET_X = 0;
            private const float CLUBS_TEXTURE_OFFSET_Y = 0;
            private const float SPADES_TEXTURE_OFFSET_X = 0.5f;
            private const float SPADES_TEXTURE_OFFSET_Y = 0;
            private const float HEARTS_TEXTURE_OFFSET_X = 0;
            private const float HEARTS_TEXTURE_OFFSET_Y = 0.25f;
            private const float DIAMONDS_TEXTURE_OFFSET_X = 0.5f;
            private const float DIAMONDS_TEXTURE_OFFSET_Y = 0.25f;
            private const float HIGH_TEXTURE_OFFSET_X = 0;
            private const float HIGH_TEXTURE_OFFSET_Y = 0.5f;
            private const float LOW_TEXTURE_OFFSET_X = 0.5f;
            private const float LOW_TEXTURE_OFFSET_Y = 0.5f;

            private float foreTextureX1;
            private float ForeTextureX1
            {
                get { return foreTextureX1; }
                set { foreTextureX1 = value; }
            }
            private float foreTextureX2;
            private float ForeTextureX2
            {
                get { return foreTextureX2; }
                set { foreTextureX2 = value; }
            }
            private float foreTextureY1;
            private float ForeTextureY1
            {
                get { return foreTextureY1; }
                set { foreTextureY1 = value; }
            }
            private float foreTextureY2;
            private float ForeTextureY2
            {
                get { return foreTextureY2; }
                set { foreTextureY2 = value; }
            }

            private float foreScreenX1;
            private float ForeScreenX1
            {
                get { return foreScreenX1; }
                set { foreScreenX1 = value; }
            }
            private float foreScreenX2;
            private float ForeScreenX2
            {
                get { return foreScreenX2; }
                set { foreScreenX2 = value; }
            }
            private float foreScreenY1;
            private float ForeScreenY1
            {
                get { return foreScreenY1; }
                set { foreScreenY1 = value; }
            }
            private float foreScreenY2;
            private float ForeScreenY2
            {
                get { return foreScreenY2; }
                set { foreScreenY2 = value; }
            }

            #endregion

            static OpenGLToggle () {
                toggleTexture = Texture .LoadTexture ("SquareToggle.png" , 145 , 145 );
                suitTexture = Texture.LoadTexture("Suits.png", 71, 62);
            }

            public OpenGLToggle(float x, float y, float width, float height, string text, int fontSize, OpenGLTableScreen parent,
                StandardHandler mouseClick, StandardHandler mouseDown, StandardHandler mouseUp, StandardHandler reset)
                : base (x , y , width , height , text , fontSize, parent, mouseClick, mouseDown, mouseUp, reset)
            {
                State = ToggleState.STANDARD;
            }

            public OpenGLToggle(float x, float y, float height, string text, int fontSize, OpenGLTableScreen parent,
                StandardHandler mouseClick, StandardHandler mouseDown , StandardHandler mouseUp, StandardHandler reset)
                : this(x, y, height, height, text, fontSize, parent, mouseClick, mouseDown, mouseUp, reset)
            {
            }

            public OpenGLToggle(float x, float y, float width, float height, string text, int fontSize, OpenGLTableScreen parent)
                : this(x, y, width, height, text, fontSize, parent, null, null, null, null)
            {
            }

            public OpenGLToggle(float x, float y, float height, string text, int fontSize, OpenGLTableScreen parent)
                : this(x, y, height, height, text, fontSize, parent, null, null, null, null)
            {
            }

            /// <summary>
            /// Draw entire toggle button.
            /// IMPORTANT: This method binds textures.  If drawing multiple toggles, use DrawBackground and DrawForeground.
            /// </summary>
            public override void Draw()
            {
                GL.BindTexture(TextureTarget.Texture2D, BackgroundTextureID);
                DrawBackground();
                GL.BindTexture(TextureTarget.Texture2D, ForegroundTextureID);
                DrawForeground();
            }

            /// <summary>
            /// Draw the background of a toggle button.
            /// IMPORTANT: Bind the appropriate texture before calling this method.
            /// </summary>
            public void DrawBackground()
            {
                if (ScreenWidth == ScreenHeight) base.Draw(); // no stretching involved so draw normally
                else
                {
                    // Crude middle-stretching algorithm
                    float TEXTURE_MIDPOINT_A = TextureX1 + toggleTexture.TextureWidth * 0.3f;
                    float TEXTURE_MIDPOINT_B = TextureX2 - toggleTexture.TextureWidth * 0.3f;
                    float SCREEN_ASPECT = ScreenWidth / ScreenHeight;
                    float SCREEN_MIDPOINT_A = ScreenX1 + ScreenWidth * 0.3f / SCREEN_ASPECT;
                    float SCREEN_MIDPOINT_B = ScreenX2 - ScreenWidth * 0.3f / SCREEN_ASPECT;

                    GL.Begin(BeginMode.Quads);

                    GL.TexCoord2(TextureX1, TextureY1);
                    GL.Vertex2(ScreenX1, ScreenY1);
                    GL.TexCoord2(TextureX1, TextureY2);
                    GL.Vertex2(ScreenX1, ScreenY2);
                    GL.TexCoord2(TEXTURE_MIDPOINT_A, TextureY2);
                    GL.Vertex2(SCREEN_MIDPOINT_A, ScreenY2);
                    GL.TexCoord2(TEXTURE_MIDPOINT_A, TextureY1);
                    GL.Vertex2(SCREEN_MIDPOINT_A, ScreenY1);

                    GL.TexCoord2(TEXTURE_MIDPOINT_A, TextureY1);
                    GL.Vertex2(SCREEN_MIDPOINT_A, ScreenY1);
                    GL.TexCoord2(TEXTURE_MIDPOINT_A, TextureY2);
                    GL.Vertex2(SCREEN_MIDPOINT_A, ScreenY2);
                    GL.TexCoord2(TEXTURE_MIDPOINT_B, TextureY2);
                    GL.Vertex2(SCREEN_MIDPOINT_B, ScreenY2);
                    GL.TexCoord2(TEXTURE_MIDPOINT_B, TextureY1);
                    GL.Vertex2(SCREEN_MIDPOINT_B, ScreenY1);

                    GL.TexCoord2(TEXTURE_MIDPOINT_B, TextureY1);
                    GL.Vertex2(SCREEN_MIDPOINT_B, ScreenY1);
                    GL.TexCoord2(TEXTURE_MIDPOINT_B, TextureY2);
                    GL.Vertex2(SCREEN_MIDPOINT_B, ScreenY2);
                    GL.TexCoord2(TextureX2, TextureY2);
                    GL.Vertex2(ScreenX2, ScreenY2);
                    GL.TexCoord2(TextureX2, TextureY1);
                    GL.Vertex2(ScreenX2, ScreenY1);

                    GL.End();
                }
            }

            /// <summary>
            /// Draw the foreground of a toggle button.
            /// IMPORTANT: Bind the appropriate texture before calling this method.
            /// </summary>
            public void DrawForeground()
            {
                if (Text != string.Empty)
                {
                    //font.FaceSize(FontSize, FONT_RESOLUTION);
                    font = new System.Drawing.Font(fontFamily, FontSize);
                    Parent.drawText(Text, x, y, true);
                }
                else if (Icon != null)
                {
                    GL.Begin(BeginMode.Quads);

                    GL.TexCoord2(ForeTextureX1, ForeTextureY1);
                    GL.Vertex2(ForeScreenX1, ForeScreenY1);
                    GL.TexCoord2(ForeTextureX1, ForeTextureY2);
                    GL.Vertex2(ForeScreenX1, ForeScreenY2);
                    GL.TexCoord2(ForeTextureX2, ForeTextureY2);
                    GL.Vertex2(ForeScreenX2, ForeScreenY2);
                    GL.TexCoord2(ForeTextureX2, ForeTextureY1);
                    GL.Vertex2(ForeScreenX2, ForeScreenY1);

                    GL.End();
                }
            }

            /// <summary>
            /// Delete any stored texture data.  IMPORTANT: Textures belonging to this class will become unusable.
            /// </summary>
            public static void DeleteTextures()
            {
                toggleTexture.DeleteTextureData();
                suitTexture.DeleteTextureData();
            }

            protected override void RecalculateBounds()
            {
                base.RecalculateBounds();

                float halfWidth = ScreenWidth * FOREGROUND_RELATIVE_SIZE / 2;
                ForeScreenX1 = X - halfWidth;
                ForeScreenX2 = X + halfWidth;

                float halfHeight = ScreenHeight * FOREGROUND_RELATIVE_SIZE / 2;
                ForeScreenY1 = Y - halfHeight;
                ForeScreenY2 = Y + halfHeight;
            }
        }

        private class OpenGLCard : OpenGLControl
        {
            private static Texture cardTexture;
            public static int TextureID
            {
                get { return cardTexture.TextureID; }
            }
            public static float AspectRatio
            {
                get { return cardTexture.AspectRatio; }
            }

            private static Texture highlightTexture;
            public static Texture HighlightTexture
            {
                get { return highlightTexture; }
            }

            private Card card;
            public Card Card
            {
                get { return card; }
                set { card = value; }
            }

            private bool highlight;
            public bool Highlight
            {
                get { return highlight; }
                set { highlight = value; }
            }

            //public override float ScreenWidth
            //{
            //    get
            //    {
            //        return base.ScreenWidth;
            //    }
            //    set
            //    {
            //        highlightScreenX1 = X - value * RELATIVE_HIGHLIGHT_SIZE_Y * HighlightTexture.AspectRatio / 2;
            //        highlightScreenX2 = X + value * RELATIVE_HIGHLIGHT_SIZE_Y * HighlightTexture.AspectRatio / 2;
            //        base.ScreenWidth = value;
            //    }
            //}
            //public override float ScreenHeight
            //{
            //    get
            //    {
            //        return base.screenHeight;
            //    }
            //    set
            //    {
            //        highlightScreenY1 = Y - value * RELATIVE_HIGHLIGHT_SIZE_Y / 2;
            //        highlightScreenY2 = Y + value * RELATIVE_HIGHLIGHT_SIZE_Y / 2;
            //        base.ScreenHeight = value;
            //    }
            //}

            protected override float TextureX1
            {
                get
                {
                    return card.textureX1;
                }
            }
            protected override float TextureX2
            {
                get
                {
                    return card.textureX2;
                }
            }
            protected override float TextureY1
            {
                get
                {
                    return card.textureY1;
                }
            }
            protected override float TextureY2
            {
                get
                {
                    return card.textureY2;
                }
            }

            protected static float highlightTextureX1 = 0;
            public static float HighlightTextureX1
            {
                get
                {
                    return highlightTextureX1;
                }
            }
            protected static float highlightTextureX2 = 0;
            public static float HighlightTextureX2
            {
                get
                {
                    return highlightTextureX2;
                }
            }
            protected static float highlightTextureY1 = 0;
            public static float HighlightTextureY1
            {
                get
                {
                    return highlightTextureY1;
                }
            }
            protected static float highlightTextureY2 = 0;
            public static float HighlightTextureY2
            {
                get
                {
                    return highlightTextureY2;
                }
            }

            public static float RELATIVE_HIGHLIGHT_SIZE_X = 242f / 150f;

            protected float highlightScreenX1 = 0;
            public float HighlightScreenX1
            {
                get { return highlightScreenX1; }
            }
            protected float highlightScreenX2 = 0;
            public float HighlightScreenX2
            {
                get { return highlightScreenX2; }
            }
            protected float highlightScreenY1 = 0;
            public float HighlightScreenY1
            {
                get { return highlightScreenY1; }
            }
            protected float highlightScreenY2 = 0;
            public float HighlightScreenY2
            {
                get { return highlightScreenY2; }
            }

            static OpenGLCard()
            {
                cardTexture = Texture.LoadTexture("cards.png", 77, 115);
                highlightTexture = Texture.LoadTexture("highlight.png", 242, 317);
                highlightTextureX1 = 0 ;
                highlightTextureX2 = HighlightTexture.PixelWidth / HighlightTexture .CanvasWidth;
                highlightTextureY1 = 0 ;
                highlightTextureY2 = HighlightTexture .PixelHeight / HighlightTexture.CanvasHeight;
            }

            public OpenGLCard()
                : base()
            {
            }

            public OpenGLCard(float x, float y, float height, Card card, OpenGLTableScreen parent, StandardHandler mouseClick, StandardHandler mouseDown ,
                StandardHandler mouseUp, StandardHandler reset )
                : base(x, y, height * cardTexture.AspectRatio, height, string.Empty, 0, parent, mouseClick, mouseDown, mouseUp, reset)
            {
                this.card = card;
            }

            public OpenGLCard(float x, float y, float height, Card card, OpenGLTableScreen parent)
                : this(x, y, height, card, parent, null, null, null, null)
            {
            }

            protected override void RecalculateBounds()
            {
                base.RecalculateBounds();
                highlightScreenX1 = X - ScreenWidth * RELATIVE_HIGHLIGHT_SIZE_X / 2f;
                highlightScreenX2 = X + ScreenWidth * RELATIVE_HIGHLIGHT_SIZE_X / 2f;
                highlightScreenY1 = Y - ScreenWidth * RELATIVE_HIGHLIGHT_SIZE_X / HighlightTexture.AspectRatio / 2f;
                highlightScreenY2 = Y + ScreenWidth * RELATIVE_HIGHLIGHT_SIZE_X / HighlightTexture.AspectRatio / 2f;
            }

            public void Deal()
            {
                float ORIGIN_X = WINDOW_MIN_X;
                float ORIGIN_Y = Y;
                const int TRAVEL_TIME = 500;

                Animate(ORIGIN_X, ORIGIN_Y, X, Y, TRAVEL_TIME, 0, 1, false);
            }

            /// <summary>
            /// Delete any stored texture data.  IMPORTANT: Textures belonging to this class will become unusable.
            /// </summary>
            public static void DeleteTextures()
            {
                cardTexture.DeleteTextureData();
                highlightTexture.DeleteTextureData();
            }
        }

        internal class BoundingBox
        {
            private BoundingBox parent = null;
            public BoundingBox Parent
            {
                get { return parent; }
            }

            private float x1;
            private float x2;
            private float y1;
            private float y2;

            private List<BoundingBox> childBoxes;

            private bool enabled = true;
            public bool Enabled
            {
                get { return enabled; }
                set { enabled = value; }
            }

            //private bool visible = true;
            //public bool Visible
            //{
            //    get { return visible; }
            //    set { visible = value; }
            //}

            private StandardHandler clickHandler;
            private StandardHandler mouseDownHandler;
            private StandardHandler mouseUpHandler;
            private StandardHandler resetHandler;

            public enum ClickType { Click, Down, Up };

            private object subject;

            /// <summary>
            /// Create a new Bounding Box.
            /// </summary>
            /// <param name="left">The lowest x value of the new Bounding Box.</param>
            /// <param name="bottom">The lowest y value of the new Bounding Box.</param>
            /// <param name="right">The highest x value of the new Bounding Box.</param>
            /// <param name="top">The highest y value of the new Bounding Box.</param>
            /// <param name="handler">The delegate to execute if this box is clicked.</param>
            public BoundingBox(float left, float bottom, float right, float top,
                StandardHandler click, StandardHandler down, StandardHandler up, StandardHandler reset, object subject)
            {
                x1 = left;
                x2 = right;
                y1 = bottom;
                y2 = top;

                childBoxes = new List<BoundingBox>();
                clickHandler = click;
                mouseDownHandler = down;
                mouseUpHandler = up;
                resetHandler = reset;
                this.subject = subject;
            }

            /// <summary>
            /// Add a Bounding Box to a larger one.
            /// </summary>
            /// <param name="bb">The Bounding Box being added.</param>
            public void AddChildBox(BoundingBox bb)
            {
                if (bb.x1 < x1 || bb.x2 > x2 || bb.y1 < y1 || bb.y2 > y2)
                    throw new Exception("Tried to add a Bounding Box that didn't fit within the target.");

                List<BoundingBox> boxesToShift = new List<BoundingBox>();

                lock (childBoxes)
                {
                    foreach (BoundingBox sbb in childBoxes)
                    {
                        // if bb fits completely within another box, send it down a level.
                        if (bb.x1 > sbb.x1 && bb.x2 < sbb.x2 && bb.y1 > sbb.y1 && bb.y2 < sbb.y2)
                        {
                            sbb.AddChildBox(bb);
                            return;
                        }
                        // if Bounding Box already exists, update the delegate and return.
                        else if (bb.Equals(sbb))
                        {
                            sbb.clickHandler = bb.clickHandler;
                            return;
                        }
                        // if bb overlaps a different box, clicking becomes ambiguous - disable both boxes.
                        else if (bb.x1 < sbb.x2 && bb.x2 > sbb.x1 && bb.y1 < sbb.y2 && bb.y2 > sbb.y1)
                        {
                            bb.Enabled = false;
                            sbb.Enabled = false;
                            //throw new Exception("A Bounding Box was created that overlapped an existing one.");
                        }
                        // find any boxes that need to be put within the new box.
                        else if (sbb.x1 > bb.x1 && sbb.x2 < bb.x2 && sbb.y1 > bb.y1 && sbb.y2 < bb.y2)
                        {
                            boxesToShift.Add(sbb);
                        }
                    }

                    // bb doesn't touch any other boxes, so add it to this level.  Add any surrounded boxes to bb's sublist.
                    childBoxes.Add(bb);
                    bb.childBoxes.AddRange(boxesToShift);
                    foreach (BoundingBox sbb in boxesToShift) childBoxes.Remove(sbb);
                    bb.parent = this;
                }
            }

            /// <summary>
            /// Remove a Bounding Box with the specified coordinates.
            /// </summary>
            /// <param name="left">The lowest x value of the new Bounding Box.</param>
            /// <param name="bottom">The lowest y value of the new Bounding Box.</param>
            /// <param name="right">The highest x value of the new Bounding Box.</param>
            /// <param name="top">The highest y value of the new Bounding Box.</param>
            public void RemoveChildBox(float left, float bottom, float right, float top)
            {
                BoundingBox target = null;

                lock (childBoxes)
                {
                    foreach (BoundingBox sbb in childBoxes)
                    {
                        if (left == sbb.x1 && right == sbb.x2 && bottom == sbb.y1 && top == sbb.y2)
                            target = sbb;
                        else if (left > sbb.x1 && right < sbb.x2 && bottom > sbb.y1 && top < sbb.y2)
                            sbb.RemoveChildBox(left, bottom, right, top);
                    }
                }

                if (target != null)
                {
                    childBoxes.AddRange(target.childBoxes);
                    childBoxes.Remove(target);
                }
                else throw new Exception("Couldn't find Bounding Box to remove.");
            }

            /// <summary>
            /// Destroy this Bounding Box.
            /// </summary>
            public void RemoveFromParent()
            {
                if (parent != null) parent.RemoveChildBox(x1, y1, x2, y2);
            }

            /// <summary>
            /// Find the smallest Bounding Box enclosing x and y and execute its Click handler.
            /// </summary>
            /// <param name="x">Mouse x</param>
            /// <param name="y">Mouse y</param>
            public void Click(float x, float y, ClickType clickType)
            {
                foreach (BoundingBox sbb in childBoxes)
                {
                    if (x > sbb.x1 && x < sbb.x2 && y > sbb.y1 && y < sbb.y2)
                    {
                        sbb.Click(x, y, clickType);
                        return;
                    }
                }

                if (enabled)
                {
                    if (clickHandler != null && clickType == ClickType.Click) clickHandler.Invoke(new object[] { x, y, subject });
                    else if (mouseDownHandler != null && clickType == ClickType.Down)
                    {
                        depressed = this;
                        mouseDownHandler.Invoke(new object[] { x, y, subject });
                    }
                    else if (mouseUpHandler != null && clickType == ClickType.Up) mouseUpHandler.Invoke(new object[] { x, y, subject });
                }
            }

            /// <summary>
            /// Move or resize this new Bounding Box
            /// </summary>
            /// <param name="newLeft">The lowest x value of the new Bounding Box.</param>
            /// <param name="newBottom">The lowest y value of the new Bounding Box.</param>
            /// <param name="newRight">The highest x value of the new Bounding Box.</param>
            /// <param name="newTop">The highest y value of the new Bounding Box.</param>
            public void Move(float newLeft, float newBottom, float newRight, float newTop)
            {
                RemoveFromParent();
                x1 = newLeft;
                y1 = newBottom;
                x2 = newRight;
                y2 = newTop;
                if (parent != null) parent.AddChildBox(this);
            }

            /// <summary>
            /// Check if a Bounding Box contains a point.
            /// </summary>
            /// <param name="x">x value of point to test.</param>
            /// <param name="y">y value of point to test.</param>
            /// <returns>true if point is inside Bounding Box</returns>
            public bool Contains(float x, float y)
            {
                return x > x1 && x < x2 && y > y1 && y < y2;
            }

            /// <summary>
            /// Cancel a button-down event
            /// </summary>
            public void Reset()
            {
                resetHandler.Invoke(new object[] { subject });
            }

            public override bool Equals(object obj)
            {
                if (obj.GetType() != typeof(BoundingBox)) return false;

                BoundingBox bb = (BoundingBox)obj;

                return (bb.x1 == x1 && bb.x2 == x2 && bb.y1 == y1 && bb.y2 == y2) ? true : false;
            }

            public override int GetHashCode()
            {
                return (int)Math.Round(x1 * 1000 + x2 * 100 + y1 * 10 + y2);
            }
        }

        private class Bool
        {
            private bool val;
            public bool Value
            {
                get { return val; }
                set { val = value; }
            }

            public Bool(bool value)
            {
                val = value;
            }

            public void Flip()
            {
                val = !val;
            }
        }

        #endregion
    }
}