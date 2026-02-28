using System;
using Raylib_cs;
using System.Numerics;
using System.Collections.Generic;
using System.Linq.Expressions;



namespace Missle 
{
    class Program 
    {
        public static float FindAngle(Vector2 item1, Vector2 item2, Vector2 direction)
        {
            direction = item2 - item1; // subtract the two points to the get the distances
            return MathF.Atan2(direction.Y, direction.X) * 180f/MathF.PI; //use the inverse tan function to find angle
        }
        public static Vector2 Move(Vector2 bullet, Vector2 forward, float speed, float dt)
        {
        // only direction
        forward = Vector2.Normalize(forward);
        // bulletPos added to forward vector and speed with time rate to be frame independent
        return bullet + forward * speed * dt;
        }

        public static float DistanceV(Vector2 A, Vector2 B)
        {
        // find the distance between two vectors on the X and Y plane
        return (float)(Math.Sqrt(Math.Pow(A.X - B.X, 2) + Math.Pow(A.Y - B.Y, 2)));
        }

        public static Vector2 FollowP(Vector2 target, Vector2 follower, float dx, float dy, double follow)
        {
            // follow player
            dx = target.X - follower.X;
            dy = target.Y - follower.Y;
            follow = Math.Atan2(dy, dx);
            follower.X += (float)(Math.Cos(follow));
            follower.Y += (float)(Math.Sin(follow));
            return follower;
        }

        public static void Main()
        {
            // INIT WINDOW
            const int WINHEIGHT = 500;
            const int WINWIDTH = 500;
            Raylib.InitWindow(WINWIDTH, WINHEIGHT, "ROCKETZ!"); // make the window
            // GAME FUNCTIONS
            Raylib.SetTargetFPS(60); // target FPS
            Raylib.HideCursor(); // hiding the cursor
            // GAME ON
            bool gameOn = false; // starting the game
            // PLAYER VARIABLES
            Texture2D playerTex = Raylib.LoadTexture("player.png"); // player Texture
            Vector2 playerPos = new(0f,350f); // playerPosition
            Vector2 playerSize = new(playerTex.Width, playerTex.Height); // playerSize based on texture
            int speed = 6; // player Movement Speeed
            // BULLET VARIABLES
            //------Missle------//
            const int MAXBULLETS = 10; // max bullets
            List<Vector2> bullets = []; // store all the bullets
            Texture2D bulletTex = Raylib.LoadTexture("missle.png");//bullet texture
            Dictionary<int, Rectangle> accessRB = [];
            // GRAVITY VARIABLES
            float gravity = 75f; // grav force
            float velocityY = 0f; // velocity
            bool onGround = true; // if player is on the ground
            // FUNCTION VARIABLES
            float dt = Raylib.GetFrameTime(); // to make it frame rate independent
            float dy = 0f; // for follow method
            float dx = 0f; // for follow method
            float follow = 0f; // for follow method
            Vector2 dir = new(); // direction vector to find angle
            // BACKGROUND
            Texture2D backgroundTex = Raylib.LoadTexture("sky.png"); // background Texture
            // TARGET
            Texture2D targetTex = Raylib.LoadTexture("target.png"); // target texture
            Vector2 targetPos1 = new(150, 75-((targetTex.Height*0.5f)/2)); // targetPos
            Vector2 targetPos2 = new(150, 180-((targetTex.Height*0.5f)/2));
            float target1Speed = 5f; // target Speeds
            float target2Speed = 1f;
            int randVal = Raylib.GetRandomValue(0, 10); // also for target speed
            // SCORE
            int score = 0;
            string scoreS = "" + score;
            // GAME LOOP
            while(!Raylib.WindowShouldClose())
            {
                // PLAYER MOVEMENT
                if (Raylib.IsKeyDown(KeyboardKey.A)) playerPos.X -= speed;
                if (Raylib.IsKeyDown(KeyboardKey.D)) playerPos.X += speed;
                if (Raylib.IsKeyPressed(KeyboardKey.W) && onGround == true) velocityY = -10f; onGround = false;
                // ONLY IF GAME IS ON
                if (gameOn == true)
                {
                // PLAYER GRAVITY
                dt = Raylib.GetFrameTime(); // frame time
                velocityY += gravity * dt; // velocity for gravity
                playerPos.Y += velocityY * gravity * dt; // add gravity to player
                // GRAVITY CLAMP
                if (playerPos.Y >= 400 - playerSize.Y) // check player Y
                {
                    velocityY = 0.0f; // setting variables
                    playerPos.Y = 400 - playerSize.Y;
                    onGround = true;
                }
                // SHOOTING MECHANISM
                if (Raylib.IsMouseButtonPressed(MouseButton.Left) || Raylib.IsKeyPressed(KeyboardKey.Space))
                {
                    Vector2 b = playerPos; // make new bullet
                    Rectangle r = new(b, new(targetTex.Width, targetTex.Height));
                    bullets.Add(b); // add to list 
                    accessRB.TryAdd(bullets.Count-1, r);         
                }
                // TARGET MOVEMENT
                if (targetPos1.X >= 350) target1Speed *= -1; // checking target 1/2 >= 350
                if (targetPos2.X >= 350) target2Speed *= -1; // inverse speed
                if (targetPos1.X <= 50) target1Speed *= -1; // checking target 1/2 <= 50
                if (targetPos2.X <= 50) target2Speed *= -1; // inverse speed
                targetPos1.X += target1Speed; // add by speed
                targetPos2.X += target2Speed;
                if (score > randVal) // if score is greater than a random value
                {
                    randVal = score + Raylib.GetRandomValue(0, 5); // change threshold
                    target1Speed += Raylib.GetRandomValue(-2, 12); // randomize speeds
                    target2Speed += Raylib.GetRandomValue(-5, 5);
                }
                }// END OF GAME ON LOOP
                // <-----DRAWING----->
                Raylib.BeginDrawing();
                // DRAW THE BACKGROUND
                Raylib.DrawTexture(backgroundTex, 0, 0, Color.White); // background
                // DRAW THE SCORE
                scoreS = ""+score;
                Raylib.DrawText(scoreS, (WINWIDTH-(Raylib.MeasureText(scoreS, 175)))/2, 215, 175, Color.LightGray); // score
                // DRAW THE PLAYER
                Raylib.DrawTextureV(playerTex, playerPos, Color.White); // player
                // DRAW THE TARGETS
                Raylib.DrawTextureEx(targetTex, targetPos2, 0f, 0.65f, Color.Gold);
                Raylib.DrawTextureEx(targetTex, targetPos1, 0f, 0.65f, Color.White);
                // DRAW THE CROSSHAIR=================================================> 
                Raylib.DrawLineEx(new(Raylib.GetMouseX() - 10, Raylib.GetMouseY()), new(Raylib.GetMouseX() + 10, Raylib.GetMouseY()), 4f, Color.Black);
                Raylib.DrawLineEx(new(Raylib.GetMouseX(), Raylib.GetMouseY() - 10), new(Raylib.GetMouseX(), Raylib.GetMouseY() + 10), 4f, Color.Black);
                // DRAW EACH OF THE BULLETS
                for (int i = 0; i < bullets.Count; i++) // iterate through bullet list
                { 
                    bullets[i] = FollowP(Raylib.GetMousePosition(), bullets[i], dy, dx, follow); // follow the mouse
                    accessRB[i] = new(bullets[i], new(targetTex.Width, targetTex.Height));
                    Raylib.DrawTextureEx(bulletTex, bullets[i], FindAngle(bullets[i], Raylib.GetMousePosition(), dir), 
                    0.75f, Color.White); // draw bullet
                    if (bullets.Count >= MAXBULLETS)
                    { 
                        bullets.RemoveAt(i); // regulate bullet size
                    }
                    // DISTANCE FROM BULLET TO MOUSE
                    if (DistanceV(Raylib.GetMousePosition(), bullets[i]) < 1f)
                    {

                        bullets.Remove(bullets[i]);
                    }  // BULLET TO TARGET2
                    else if (Raylib.CheckCollisionCircleRec(targetPos2, targetTex.Width*0.65f, accessRB[i]))
                    {
                        bullets.Remove(bullets[i]);
                        score--; // lower score
                    }  // BULLET TO TARGET1
                    else if (Raylib.CheckCollisionCircleRec(targetPos1, targetTex.Width*0.65f, accessRB[i]))
                    {
                        bullets.Remove(bullets[i]);
                        score++; // more score
                    } 
                
                    
                }
                // PRE GAME
                if (gameOn == false)
                {
                 if (Raylib.IsMouseButtonPressed(MouseButton.Left)) gameOn = true;   
                 Raylib.DrawText("CLICK TO START", (WINWIDTH-(Raylib.MeasureText("CLICK TO START", 50)))/2, 
                 (WINWIDTH-(50))/2, 50, Color.White); // click to start
                }
                Raylib.EndDrawing(); // end the drawing
            }
            Raylib.CloseWindow(); // close the window after game loop
            // UNLOADING TEXTURES
            Raylib.UnloadTexture(playerTex);
            Raylib.UnloadTexture(bulletTex);
            Raylib.UnloadTexture(backgroundTex);
            Raylib.UnloadTexture(targetTex);
        }
    }
}
