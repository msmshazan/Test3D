using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Test3D
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class TestGame : Game
    {
        GraphicsDeviceManager Graphics;
        SpriteBatch SpriteBatch;
        SpriteFont Font;
        Matrix View;
        Matrix World;
        Matrix Projection;
        Vector3 CameraTarget;
        Vector3 CameraPos;
        BasicEffect BasicEffect;
        Texture2D Pixel;
        Vector2 MousePosDragStart;
        Vector2 MousePosDragEnd;
        bool IsDragging;
        bool IsPrevMousePress;
        public TestGame()
        {
            Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        
        protected override void Initialize()
        {
            CameraPos = new Vector3(0, 0, 50);
            CameraTarget = new Vector3(0,0,5);
            BasicEffect = new BasicEffect(GraphicsDevice);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            Font = Content.Load<SpriteFont>("Font");
            /*
            View = Matrix.Identity;
            Projection = Matrix.CreateOrthographicOffCenter(-16 * GraphicsDevice.Viewport.AspectRatio, 16* GraphicsDevice.Viewport.AspectRatio,-16,16 , -100f, 100f);
            World = Matrix.Identity;
            */
            View = Matrix.CreateLookAt(CameraPos,CameraTarget,Vector3.Up);
            Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, GraphicsDevice.Viewport.AspectRatio, 0.1f, 100f);
            World = Matrix.Identity;
           
            Pixel = new Texture2D(GraphicsDevice, 1, 1);
            Pixel.SetData(new[] { Color.White });
        }

       
        protected override void UnloadContent()
        {
            
        }

        
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            bool MousePressed = (Mouse.GetState().LeftButton == ButtonState.Pressed);
            var MP = Mouse.GetState().Position.ToVector2();
            var SP = Window.ClientBounds.Size.ToVector2();
            MP.X = (2 * MP.X) - SP.X;
            MP.Y = -((2 * MP.Y) - SP.Y);
            var NewMousePosCoord = MP / SP;
            NewMousePosCoord.X = MathHelper.Clamp(NewMousePosCoord.X, -1, 1);
            NewMousePosCoord.Y = MathHelper.Clamp(NewMousePosCoord.Y, -1, 1);
            IsDragging = MousePressed && IsPrevMousePress;
            if (!IsDragging)
            {
                MousePosDragEnd = MousePosDragStart = Vector2.Zero;
            }
            var IsDragStart = MousePressed && (!IsPrevMousePress);
            if (IsDragStart)
            {
                MousePosDragStart = NewMousePosCoord;
            }
            var IsDragEnd = (!MousePressed) && IsPrevMousePress;
            if (IsDragging)
            {
                MousePosDragEnd = NewMousePosCoord;
                var va = GetArcBallQuat(MousePosDragStart*0.02f);
                var vb = GetArcBallQuat(MousePosDragEnd*0.02f);
                View =  Matrix.CreateFromQuaternion(vb*va)*View;
                
            }
            IsPrevMousePress = MousePressed;
            BasicEffect.World = World;
            BasicEffect.View = View;
            BasicEffect.Projection = Projection;
            BasicEffect.CurrentTechnique = BasicEffect.Techniques["BasicEffect_Texture_VertexColor_NoFog"];
            BasicEffect.Texture = Pixel;
            BasicEffect.TextureEnabled = true;
            BasicEffect.FogEnabled = false;
            base.Update(gameTime);
        }

        private Quaternion GetArcBallQuat(Vector2 mousePosition)
        {
            var dist= Vector2.Dot(mousePosition, mousePosition);
            if(dist <= 1)
            {
                return new Quaternion(0, mousePosition.X,mousePosition.Y, (float)Math.Sqrt(1 - dist));
            }
            else
            {
                mousePosition.Normalize();
                return new Quaternion(0, mousePosition.X, mousePosition.Y, 0);
            }
        
        }
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            GraphicsDevice.RasterizerState = RasterizerState.CullNone;
            var vertices = new VertexPositionColorTexture[] {
                new VertexPositionColorTexture(new Vector3(-8,-8,5),Color.White,new Vector2(0,0)),
                new VertexPositionColorTexture(new Vector3(8,8,5),Color.White,new Vector2(1,1)),
                new VertexPositionColorTexture(new Vector3(8,-8,5),Color.White,new Vector2(1,0)),
                new VertexPositionColorTexture(new Vector3(-8,-8,5),Color.White,new Vector2(0,0)),
                new VertexPositionColorTexture(new Vector3(-8,8,5),Color.White,new Vector2(0,1)),
                new VertexPositionColorTexture(new Vector3(8,8,5),Color.White,new Vector2(1,1))
            };

            foreach (var pass in BasicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList,vertices, 0, 2);
            }
            SpriteBatch.Begin();
            SpriteBatch.DrawString(Font, $"MouseCoordDragStart: {MousePosDragStart.ToString()}", new Vector2(20, 20), Color.White);
            SpriteBatch.DrawString(Font, $"MouseCoordDragEnd: {MousePosDragEnd.ToString()}", new Vector2(20, 40), Color.White);
            SpriteBatch.DrawString(Font, $"IsDragging: {IsDragging.ToString()}", new Vector2(20, 60), Color.White);
            SpriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
