using System;
using Google.OrTools.LinearSolver;
using SplashKitSDK;

namespace solver_tutorial
{
    class Program
    {
        static void Main(string[] args)
        {
            // Initialize SplashKit
            SplashKit.OpenWindow("Farmer's Crop Optimisation", 800, 600);

            // Variables for user input
            double land = 14; // Total land available (acres)
            double labor = 10; // Total labor available (hours)
            double wheatProfit = 3; // Profit per acre of wheat ($)
            double barleyProfit = 4; // Profit per acre of barley ($)

            // Variables to track if the problem has been solved
            bool isSolved = false;
            string resultText = "";

            // Main loop
            while (!SplashKit.WindowCloseRequested("Farmer's Crop Optimisation"))
            {
                // Clear the screen
                SplashKit.ClearScreen(Color.White);

                // Draw instructions
                SplashKit.DrawText("Farmer's Crop Optimisation", Color.Black, 50, 50);
                SplashKit.DrawText("Use Up/Down arrows to adjust land values.", Color.Blue, 50, 80);
                SplashKit.DrawText("Use Left/Right arrows to adjust labor values.", Color.Blue, 50, 110);
                SplashKit.DrawText("Use W/S arrows to wheat profit values.", Color.Blue, 50, 140);
                SplashKit.DrawText("Use B/N arrows to barley profit values.", Color.Blue, 50, 170);
                SplashKit.DrawText("Press SPACE to optimise.", Color.Blue, 50, 200);

                // Display current values
                SplashKit.DrawText($"Land (acres): {land}", Color.Black, 50, 260);
                SplashKit.DrawText($"Labor (hours): {labor}", Color.Black, 50, 290);
                SplashKit.DrawText($"Wheat Profit ($/acre): {wheatProfit}", Color.Black, 50, 320);
                SplashKit.DrawText($"Barley Profit ($/acre): {barleyProfit}", Color.Black, 50, 350);

                // Handle user input
                if (SplashKit.KeyTyped(KeyCode.UpKey))
                {
                    land = Math.Max(0, land + 1); // Increase land
                    isSolved = false; // Reset solved state
                }
                if (SplashKit.KeyTyped(KeyCode.DownKey))
                {
                    land = Math.Max(0, land - 1); // Decrease land
                    isSolved = false; // Reset solved state
                }
                if (SplashKit.KeyTyped(KeyCode.RightKey))
                {
                    labor = Math.Max(0, labor + 1); // Increase labor
                    isSolved = false; // Reset solved state
                }
                if (SplashKit.KeyTyped(KeyCode.LeftKey))
                {
                    labor = Math.Max(0, labor - 1); // Decrease labor
                    isSolved = false; // Reset solved state
                }
                if (SplashKit.KeyTyped(KeyCode.WKey))
                {
                    wheatProfit = Math.Max(0, wheatProfit + 1); // Increase wheat profit
                    isSolved = false; // Reset solved state
                }
                if (SplashKit.KeyTyped(KeyCode.SKey))
                {
                    wheatProfit = Math.Max(0, wheatProfit - 1); // Decrease wheat profit
                    isSolved = false; // Reset solved state
                }
                if (SplashKit.KeyTyped(KeyCode.BKey))
                {
                    barleyProfit = Math.Max(0, barleyProfit + 1); // Increase barley profit
                    isSolved = false; // Reset solved state
                }
                if (SplashKit.KeyTyped(KeyCode.NKey))
                {
                    barleyProfit = Math.Max(0, barleyProfit - 1); // Decrease barley profit
                    isSolved = false; // Reset solved state
                }

                // Solve the problem when SPACE is pressed
                if (SplashKit.KeyTyped(KeyCode.SpaceKey))
                {
                    // Create a linear solver using the GLOP backend
                    Solver solver = Solver.CreateSolver("GLOP");

                    // Create variables
                    Variable wheat = solver.MakeNumVar(0.0, double.PositiveInfinity, "wheat");
                    Variable barley = solver.MakeNumVar(0.0, double.PositiveInfinity, "barley");

                    // Add constraints
                    solver.Add(wheat + barley <= land); // Land constraint
                    solver.Add(wheat + 2 * barley <= labor); // Labor constraint

                    // Set the objective function: maximize profit
                    solver.Maximize(wheatProfit * wheat + barleyProfit * barley);

                    // Solve the problem
                    Solver.ResultStatus resultStatus = solver.Solve();

                    // Store the results
                    if (resultStatus == Solver.ResultStatus.OPTIMAL)
                    {
                        resultText = $"Optimal solution found!\n" +
                                     $"Wheat to plant: {wheat.SolutionValue():F2} acres\n" +
                                     $"Barley to plant: {barley.SolutionValue():F2} acres\n" +
                                     $"Total profit: ${solver.Objective().Value():F2}";
                        isSolved = true;
                    }
                    else
                    {
                        resultText = "No optimal solution found.";
                        isSolved = true;
                    }
                }

                // Display the results if solved
                if (isSolved)
                {
                    string[] resultLines = resultText.Split('\n');
                    int yOffset = 450; // Starting Y position for results
                    foreach (string line in resultLines)
                    {
                        SplashKit.DrawText(line, Color.Black, 50, yOffset);
                        yOffset += 30; // Move down for the next line
                    }
                }

                // Draw the constraints and feasible region
                DrawConstraints(land, labor, 500, 200);

                // Refresh the screen
                SplashKit.RefreshScreen();
                SplashKit.ProcessEvents();
            }
        }

        static void DrawConstraints(double land, double labor, int graphX = 500, int graphY = 200)
        {
            // Define graph dimensions
            int graphWidth = 250;
            int graphHeight = 250;
            
            // Calculate dynamic scale based on current values
            double maxX = Math.Max(land, labor);
            double maxY = Math.Max(land, labor/2);
            double xScale = graphWidth / (maxX * 1.2);  // Add 20% padding
            double yScale = graphHeight / (maxY * 1.2);

            // Draw the graph background
            SplashKit.FillRectangle(Color.LightGray, graphX, graphY, graphWidth, graphHeight);
            SplashKit.DrawRectangle(Color.Black, graphX, graphY, graphWidth, graphHeight);

            // Draw axes
            SplashKit.DrawLine(Color.Black, graphX, graphY + graphHeight, graphX + graphWidth, graphY + graphHeight); // X-axis
            SplashKit.DrawLine(Color.Black, graphX, graphY, graphX, graphY + graphHeight); // Y-axis

            // Draw axis labels with clear context
            // X-axis label (moved further down)
            SplashKit.DrawText($"Wheat (x <= {land})", 
                Color.Black, 
                graphX + graphWidth/2 - 50,  // Centered
                graphY + graphHeight + 10);   // Below graph

            // Y-axis label (rotated and moved left)
            SplashKit.DrawText($"Barley (y <= {labor/2})", 
                Color.Black, 
                graphX - 150, 
                graphY + graphHeight/2 - 10);

            // Add constraint equations
            SplashKit.DrawText($"Land: x + y <= {land}", 
                Color.Blue, 
                graphX + 10, graphY + 10);
            SplashKit.DrawText($"Labor: x + 2y <= {labor}", 
                Color.Red, 
                graphX + 10, graphY + 30);


            // Draw the land constraint with boundary checking
            double landX2 = land;
            double landY1 = land;
            if(landY1 > maxY * 1.2) landY1 = maxY * 1.2;
            if(landX2 > maxX * 1.2) landX2 = maxX * 1.2;

            SplashKit.DrawLine(Color.Blue,
                graphX + (int)(0 * xScale), 
                graphY + graphHeight - (int)(landY1 * yScale),
                graphX + (int)(landX2 * xScale), 
                graphY + graphHeight - (int)(0 * yScale));

            // Draw labor constraint with boundary checking
            double laborY1 = labor/2;
            double laborX2 = labor;
            if(laborY1 > maxY * 1.2) laborY1 = maxY * 1.2;
            if(laborX2 > maxX * 1.2) laborX2 = maxX * 1.2;

            SplashKit.DrawLine(Color.Red,
                graphX + (int)(0 * xScale),
                graphY + graphHeight - (int)(laborY1 * yScale),
                graphX + (int)(laborX2 * xScale),
                graphY + graphHeight - (int)(0 * yScale));

            // Calculate intersection point with boundary checks
            double intersectionX = (2 * land - labor);
            double intersectionY = (labor - land);
            
            // Clamp values to graph bounds
            intersectionX = Math.Max(0, Math.Min(intersectionX, maxX * 1.2));
            intersectionY = Math.Max(0, Math.Min(intersectionY, maxY * 1.2));

            // Define feasible region vertices with clamping
            Point2D[] feasibleRegion = new Point2D[]
            {
                new Point2D() { X = graphX, Y = graphY + graphHeight },
                new Point2D() { 
                    X = graphX + (int)(intersectionX * xScale), 
                    Y = graphY + graphHeight - (int)(intersectionY * yScale) 
                },
                new Point2D() { 
                    X = graphX + (int)(Math.Min(land, maxX * 1.2) * xScale), 
                    Y = graphY + graphHeight 
                },
                new Point2D() { X = graphX, Y = graphY + graphHeight }
            };

            // Draw the feasible region only if valid
            if(intersectionX >= 0 && intersectionY >= 0)
            {
                SplashKit.FillTriangle(
                    Color.LightGreen,
                    feasibleRegion[0].X, feasibleRegion[0].Y,
                    feasibleRegion[1].X, feasibleRegion[1].Y,
                    feasibleRegion[2].X, feasibleRegion[2].Y
                );
            }
        }
    }
}