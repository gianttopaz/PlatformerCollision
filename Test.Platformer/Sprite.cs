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

    //public Sprite()
    //{
    //    velocity = new();
    //    isGrounded = false;
    //    isClinging = false;
    //    Direction = 1;
    //    isStatic = false;
    //    isFalling = false;
    //    wallJump = false;
    //}
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


        //int prevDir = Direction;

        //if (velocity.Y > 0)
        //{ 
        //    isFalling = true;
        //}
        //else isFalling = false;

        //if (isGrounded)
        //{
        //    isClinging = false;
        //}

        //isStatic = true;

        //velocity.Y += 0.5f;
        //velocity.Y = Math.Min(velocity.Y, 10.0f);


        //if (keystate.IsKeyDown(Keys.Right))
        //{
        //    velocity.X += 1.5f;
        //    velocity.X = Math.Min(velocity.X, 5);
        //    Direction = 1;
        //    isStatic = false;
        //}
        //if (!wallJump && keystate.IsKeyDown(Keys.Left))
        //{
        //    velocity.X -= 1.5f;
        //    velocity.X = Math.Min(velocity.X, -5);
        //    if (!isClinging && !wallJump)
        //    {
        //        Direction = -1;
        //    }
        //    else if (isClinging && isFalling)
        //    {
        //        Direction = 1;
        //    }
        //    isStatic = false;
        //    if ((isClinging && isFalling) && keystate.IsKeyDown(Keys.Space) && !prevKBState.IsKeyDown(Keys.Space))
        //    {
        //        wallJump = true;
        //    }
        //}

        //if (wallJump)
        //{
        //    spawn += (float)gameTime.ElapsedGameTime.TotalSeconds;
        //    velocity.X += 3f;
        //    if (spawn >= 0.15)
        //    {
        //        wallJump = false;   
        //        spawn = 0;
        //    }
        //}
        //else
        //    velocity.X = Math.Max(-5f, Math.Min(5f, velocity.X));
        ////friction
        //if (isStatic)
        //    velocity.X *= 0.7f;
        //// wall hold
        //if (isFalling && isClinging)
        //    velocity.Y = 2.5f;

        ////jump/walljump
        //if ((isGrounded || (isClinging && isFalling)) && keystate.IsKeyDown(Keys.Space) && !prevKBState.IsKeyDown(Keys.Space))
        //{
        //    velocity.Y = -8;
        //}
        ////fastfall
        //if (isFalling & keystate.IsKeyDown(Keys.Down))
        //{
        //    velocity.Y = 8;
        //}
        ////flipper
        //if (prevDir != Direction)
        //{
        //    srect.X += srect.Width;
        //    srect.Width = -srect.Width;

        //}
        //if (isGrounded)
        //{
        //    animationPlayer.PlayAnimation(idleAnimation);
        //}
        //else if (velocity.X != 0 && isGrounded)
        //{
        //    animationPlayer.PlayAnimation(runAnimation);
        //}

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

    //public void OnKilled()
    //{
    //    isAlive = false;

    //    if (killedBy != null)
    //        killedSound.Play();
    //    else
    //        fallSound.Play();

    //    sprite.PlayAnimation(dieAnimation);
    //}

    public virtual void Draw(SpriteBatch spriteBatch)
    {
        if (texture != null)
            spriteBatch.Draw(texture, Position, Color.White);
        else if (animationManager != null)
            animationManager.Draw(spriteBatch);
        else throw new Exception("This ain't right..!");
    }

}
