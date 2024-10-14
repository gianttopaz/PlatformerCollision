using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Emit;
using Test.Platformer;

namespace Test.Platformer;

public class Sprite
{

    public Texture2D texture;
    public Rectangle rect;
    public Vector2 velocity;
    public float Speed = 3f;


    Vector2 position = new Vector2(40, 350);


    private Animation idleAnimation;
    private Animation runAnimation;
    private Animation jumpAnimation;
    private Animation wallAnimation;

    AnimationManager animationManager;

    protected Dictionary<string, Animation> animations;

    public Vector2 Position
    {
        get { return position; }
        set
        {
            position = value;

            if (animationManager != null)
                animationManager.Position = position;
        }
    }    

    public bool isGrounded { get; set; }
    public bool isClinging { get; set; }
    public int Direction { get; set; }
    public bool isStatic { get; set; }
    public bool isFalling { get; set; }
    public bool wallJump;
    private float spawn = 0;

    public Sprite(Dictionary<string, Animation> animations, Rectangle rect)
    {
        this.animations = animations;
        animationManager = new AnimationManager(animations.First().Value);
        this.rect = rect;
    }

    public Sprite(Texture2D texture)
    {
        this.texture = texture;
    }


    public void Update(KeyboardState keystate, KeyboardState prevKBState, GameTime gameTime)
    {
        Move();

        SetAnimations();


        animationManager.Update(gameTime);

        Position += velocity;
        velocity = Vector2.Zero;

    }

    private void SetAnimations()
    {
        if (velocity.X > 0)
            animationManager.Play(animations["RunRight"]);
        else if (velocity.X < 0)
            animationManager.Play(animations["RunLeft"]);
        else if (velocity.Y < 0)
            animationManager.Play(animations["RunUp"]);
        else if (velocity.Y > 0)
            animationManager.Play(animations["RunDown"]);
        else animationManager.Stop();
    }

    private void Move()
    {
        if (Keyboard.GetState().IsKeyDown(Keys.Up))
            velocity.Y = -Speed;
        else if (Keyboard.GetState().IsKeyDown(Keys.Down))
            velocity.Y = Speed;
        else if (Keyboard.GetState().IsKeyDown(Keys.Left))
            velocity.X = -Speed;
        else if (Keyboard.GetState().IsKeyDown(Keys.Right))
            velocity.X = Speed;
    }

    public virtual void Draw(SpriteBatch spriteBatch)
    {
        if (texture != null)
            spriteBatch.Draw(texture, Position, Color.White);
        else if (animationManager != null)
            animationManager.Draw(spriteBatch);
        else throw new Exception("This ain't right..!");
    }

}
