using System;
//using Tao.OpenGl;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Diagnostics;
using System.Drawing;
using Img = System.Drawing.Imaging;

namespace ShootClient
{
    class Texture
    {
        private string filename;
        /// <summary>
        /// Filename of texture.
        /// </summary>
        public string Filename
        {
            get { return filename; }
        }

        private int textureID;
        /// <summary>
        /// OpenGL generated texture name.
        /// </summary>
        public int TextureID
        {
            get
            {
                if (deleted) throw new Exception("Texture has been deleted.");
                return textureID;
            }
        }

        private bool deleted = false;
        /// <summary>
        /// True if texture has been deleted, making it unusable.
        /// </summary>
        public bool Deleted
        {
            get { return deleted; }
        }

        private float aspectRatio;
        /// <summary>
        /// The aspect ratio of the image within the canvas.
        /// </summary>
        public float AspectRatio
        {
            get
            {
                if (deleted) throw new Exception("Texture has been deleted.");
                return aspectRatio;
            }
        }

        private float canvasWidth;
        /// <summary>
        /// Width of the canvas in pixels.
        /// </summary>
        public float CanvasWidth
        {
            get
            {
                if (deleted) throw new Exception("Texture has been deleted.");
                return canvasWidth;
            }
        }

        private float canvasHeight;
        /// <summary>
        /// Height of the canvas in pixels.
        /// </summary>
        public float CanvasHeight
        {
            get
            {
                if (deleted) throw new Exception("Texture has been deleted.");
                return canvasHeight;
            }
        }

        private float pixelWidth;
        /// <summary>
        /// Width of the image in pixels.
        /// </summary>
        public float PixelWidth
        {
            get
            {
                if (deleted) throw new Exception("Texture has been deleted.");
                return pixelWidth;
            }
            set
            {
                if (deleted) throw new Exception("Texture has been deleted.");

                pixelWidth = value;
                aspectRatio = pixelHeight > 0 ? pixelWidth / pixelHeight : 1;
                textureWidth = pixelWidth / canvasWidth;
            }
        }

        private float textureWidth;
        /// <summary>
        /// Width of the image in texture coordinates.
        /// </summary>
        public float TextureWidth
        {
            get
            {
                if (deleted) throw new Exception("Texture has been deleted.");
                return textureWidth;
            }
        }

        private float pixelHeight;
        /// <summary>
        /// Width of the image in pixels.
        /// </summary>
        public float PixelHeight
        {
            get
            {
                if (deleted) throw new Exception("Texture has been deleted.");
                return pixelHeight;
            }
            set
            {
                if (deleted) throw new Exception("Texture has been deleted.");

                pixelHeight = value;
                aspectRatio = pixelHeight > 0 ? pixelWidth / pixelHeight : 1;
                textureHeight = pixelHeight / canvasHeight;
            }
        }

        private float textureHeight;
        /// <summary>
        /// Height of the image in texture coordinates.
        /// </summary>
        public float TextureHeight
        {
            get
            {
                if (deleted) throw new Exception("Texture has been deleted.");
                return textureHeight;
            }
        }

        private Texture(string _filename, int _textureID, float _canvasWidth, float _canvasHeight, float _pixelWidth, float _pixelHeight)
        {
            filename = _filename;
            textureID = _textureID;
            canvasWidth = _canvasWidth;
            canvasHeight = _canvasHeight;
            PixelWidth = _pixelWidth;
            //textureWidth = pixelWidth / canvasWidth;
            PixelHeight = _pixelHeight;
            //textureHeight = pixelHeight / canvasHeight;
            //aspectRatio = pixelWidth / pixelHeight;
        }

        /// <summary>
        /// This function takes some image data as input, and creates an openGL texture object
        /// </summary>
        /// <param name="Format"> Gl.GL_RGB or Gl.GL_RGBA </param>
        /// <param name="pixels"> the pixel data </param>
        /// <param name="w"> the image width </param>
        /// <param name="h"> the image height </param>
        /// <returns> the openGL texture object </returns>
        private static uint MakeGlTexture(int Format, IntPtr pixels, int w, int h)
        {
            return 0;
            //// from http://wiki.nccaforum.com/index.php?title=Devil_tutorial
            //uint texObject;

            //// generate an openGL texture object
            //GL.GenTextures(1, out texObject);

            //// pixel data is going to be tightly packed
            ////GL.PixelStorei(Gl.GL_UNPACK_ALIGNMENT, 1);

            //// bind to our texture object we just created
            //GL.BindTexture(TextureTarget.Texture2D, texObject);
            
            //// set the filtering and wrap modes for the texture
            //GL.TexParameteri(TextureTarget.Texture2D, Gl.GL_TEXTURE_WRAP_S, Gl.GL_REPEAT);
            //GL.TexParameteri(TextureTarget.Texture2D, Gl.GL_TEXTURE_WRAP_T, Gl.GL_REPEAT);
            //GL.TexParameteri(TextureTarget.Texture2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_LINEAR_MIPMAP_LINEAR);
            //GL.TexParameteri(TextureTarget.Texture2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_LINEAR_MIPMAP_LINEAR);

            //Glu.gluBuild2DMipmaps(TextureTarget.Texture2D, Format == Gl.GL_RGBA ? 4 : 3, w, h, Format, Gl.GL_UNSIGNED_BYTE, pixels);

            ////GL.DeleteTextures(1, ref texObject);

            //// return the new texture object
            //return texObject;
        }

        public static Texture LoadTexture(string filename, float pixelWidth, float pixelHeight)
        {
            Texture result = null;

            Bitmap bitmap = new Bitmap(Bitmap.FromFile(filename));

            int canvasWidth = bitmap.Width;
            int canvasHeight = bitmap.Height;

            //TexUtil.InitTexturing();

            int texture = TexUtil.CreateTextureFromBitmap(bitmap);

            if (texture != 0) result = new Texture(filename, texture, canvasWidth, canvasHeight, pixelWidth, pixelHeight);

            GL.Enable(EnableCap.Texture2D);

            // I don't think I need this stuff
            //// Disable vertical sync (on cards that support it)
            //Glfw.glfwSwapInterval(0);
            //GL.ClearColor(0.0f, 0.0f, 0.0f, 0.0f);
            //GL.ShadeModel(Gl.GL_SMOOTH);

            //Texture result = null;

            //uint texture = 0;

            //int imageId;

            //// Initialize DevIL
            //Il.ilInit();

            //// Generate the main image name to use
            //Il.ilGenImages(1, out imageId);

            //// Bind this image name
            //Il.ilBindImage(imageId);

            //// Loads the image into the imageId
            //if (Il.ilLoadImage(filename))
            //{
            //    // get the image's dimensions
            //    int canvasWidth = Il.ilGetInteger(Il.IL_IMAGE_WIDTH);
            //    int canvasHeight = Il.ilGetInteger(Il.IL_IMAGE_HEIGHT);
            //    int bitspp = Il.ilGetInteger(Il.IL_IMAGE_BITS_PER_PIXEL);

            //    // Convert to OpenGL texture object
            //    switch (bitspp)
            //    {
            //        case 3:
            //        case 24:
            //            texture = MakeGlTexture(Gl.GL_RGB, Il.ilGetData(), canvasWidth, canvasHeight);
            //            break;
            //        case 4:
            //        case 32:
            //            texture = MakeGlTexture(Gl.GL_RGBA, Il.ilGetData(), canvasWidth, canvasHeight);
            //            break;

            //        default:
            //            break;
            //    }

            //    if (texture != 0) result = new Texture(filename, texture, canvasWidth, canvasHeight, pixelWidth, pixelHeight);
            //}
            //// Done with the imageId, so let's delete it
            //Il.ilDeleteImages(1, ref imageId);

            //// enable openGL texturing
            //GL.Enable(TextureTarget.Texture2D);

            return result;
        }

        public void DeleteTextureData()
        {
            //GL.DeleteTextures(1, ref textureID);
            //deleted = true;
            GL.DeleteTexture((int)textureID);
            deleted = true;
        }
    }
}
