using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Markup;
using System.Collections.Generic;

namespace GrafPack
{
    public partial class Form1 : Form
    {
        //Menu which shows different options after right click
        ContextMenu transformOptions = new ContextMenu(); 

        private bool selectSquareStatus = false; 
        private bool selectTriangleStatus = false; 
        private bool selectCircleStatus = false;  
        private bool shapeSelection = false; 
        private bool mouseHeld = false; // set true after 1st click in create, facilitates Rubber banding in mouseMove
        private int shapeIndex = 0;
        private int moveByDragIndex = 0; // taking the index from list of the shape which is being moved by drag
        private int rotationIndex = 0; // taking the index from list of the shape which is selected for rotation
        private bool moveByDrag = false; 
        private bool rotateBy90 = false; 
        private bool rotateBy180 = false; 

        private static List<Shape> shape = new List<Shape>(); // This will store the shapes

        private int clicknumber = 0; 
        private Point one;  
        private Point two;  
        private Point draggedMouseLocation; 
        private Point newPos = new Point(); 
        private Point rotatedKeyPt = new Point(); 
        private Point rotatedOppPt = new Point(); 
        private int differenceX = 0; 
        private int differenceY = 0; 

        public Form1()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = Color.White;

            TextBox moveByInputBox = new TextBox();
            moveByInputBox.Text = "0";

            // The following approach uses menu items coupled with mouse clicks. The main options
            MainMenu mainMenu = new MainMenu();
            MenuItem createItem = new MenuItem();
            MenuItem selectItem = new MenuItem();
            MenuItem exitProgram = new MenuItem();

            // the shape creation options
            MenuItem squareItem = new MenuItem();
            MenuItem triangleItem = new MenuItem();
            MenuItem circleItem = new MenuItem();


            // the options for selected shape
            MenuItem moveItem = new MenuItem();
            MenuItem rotateItem = new MenuItem();
            MenuItem deleteItem = new MenuItem();


            // the sub options for selected shape
            MenuItem rotate90 = new MenuItem();
            MenuItem rotate180 = new MenuItem();

            // The text on the buttons
            createItem.Text = "&Create";
            squareItem.Text = "&Square";
            triangleItem.Text = "&Triangle";
            circleItem.Text = "&Circle";
            selectItem.Text = "&Select";
            exitProgram.Text = "&Exit";

            moveItem.Text = "&Move";
            rotateItem.Text = "&Rotate";
            deleteItem.Text = "&Delete";
            rotate90.Text = "&Rotate by 90";
            rotate180.Text = "&Rotate by 180";

            moveItem.Text = "&Move Item by Mouse Drag";

            // the main options on top menu bar
            mainMenu.MenuItems.Add(createItem);
            mainMenu.MenuItems.Add(selectItem);
            mainMenu.MenuItems.Add(exitProgram);

            // adding the sub-options for create 
            createItem.MenuItems.Add(squareItem);
            createItem.MenuItems.Add(triangleItem);
            createItem.MenuItems.Add(circleItem);

            // adding the options for context menu
            transformOptions.MenuItems.Add(moveItem);
            transformOptions.MenuItems.Add(rotateItem);
            transformOptions.MenuItems.Add(deleteItem);

            // the sub options for the context menu
            rotateItem.MenuItems.Add(rotate90);
            rotateItem.MenuItems.Add(rotate180);

            // Adding a click event to the buttons, which will invoke a method
            selectItem.Click += new System.EventHandler(this.selectShape);
            squareItem.Click += new System.EventHandler(this.selectSquare);
            triangleItem.Click += new System.EventHandler(this.selectTriangle);
            circleItem.Click += new System.EventHandler(this.selectCircle);
            exitProgram.Click += new System.EventHandler(this.ExitProgram);
            rotate90.Click += new System.EventHandler(this.RotateAt90);
            rotate180.Click += new System.EventHandler(this.RotateAt180);
            moveItem.Click += new System.EventHandler(this.MoveByDrag);
            deleteItem.Click += new System.EventHandler(this.deleteShape);

            this.Menu = mainMenu;
            this.ContextMenu = transformOptions;
            this.MouseClick += mouseClick; // This is used to create shapes and move them by drag
            this.MouseDown += mouseDown; 
            this.MouseUp += mouseUp;    
            this.MouseMove += mouseMove; // used for rubber banding and moving by drag     
        }

        private void refreshScreen()
        {
            Invalidate();
            Refresh();
            redrawShapes();
        }

        private void selectSquare(object sender, EventArgs e)
        {
            selectSquareStatus = true;
            MessageBox.Show("Click on 2 different location to create a Square");
            selectTriangleStatus = false;
            selectCircleStatus = false;
        }

        private void selectTriangle(object sender, EventArgs e)
        {
            selectTriangleStatus = true;
            MessageBox.Show("Click on 2 different location to create a Triangle");
            selectSquareStatus = false;
            selectCircleStatus = false;
        }

        private void selectCircle(object sender, EventArgs e)
        {
            selectCircleStatus = true;
            MessageBox.Show("Click on 2 different location to create a Cirlce");
            selectSquareStatus = false;
            selectTriangleStatus = false;
        }

        private void selectShape(object sender, EventArgs e)
        {
            shapeSelection = true;
            MessageBox.Show("You selected the Select option, Right click to get options");
            Selection();
        }

        private void MoveByDrag(object sender, EventArgs e)
        {
            moveByDrag = true;
        }

        private void ExitProgram(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Selection()
        {
            //If statement, if no there are no shapes to choose from
            if (shape.Count == 0) 
            {
                MessageBox.Show("Error!! There are no shapes to choose from, please create shapes first");
                shapeSelection = false;
            }
            else
            {
            }
        }

        private Point RotatePt(float angle, Point Pt, Point center)
        {
            float cosa = (float)Math.Cos(angle * Math.PI / 180.0);
            float sina = (float)Math.Sin(angle * Math.PI / 180.0);

            // calculating new points
            float X = (cosa * (Pt.X - center.X) - sina * (Pt.Y - center.Y) + center.X);
            float Y = (sina * (Pt.X - center.X) + cosa * (Pt.Y - center.Y) + center.Y);
            Pt = new Point((int)X, (int)Y);
            return Pt;
        }

        private void RotateAt90(object sender, EventArgs e)
        {
            Pen blackpen = new Pen(Color.Black);
            rotateBy90 = true;
            rotationIndex = shapeIndex;
            shapeIdentify(shapeIndex, blackpen);
            refreshScreen();
        }

        private void RotateAt180(object sender, EventArgs e)
        {
            Pen blackpen = new Pen(Color.Black);
            rotateBy180 = true;
            rotationIndex = shapeIndex;
            shapeIdentify(shapeIndex, blackpen);
            refreshScreen();
        }

        private void deleteShape(object sender, EventArgs e)
        {
            shape.RemoveAt(shapeIndex);
            refreshScreen();
        }

        private void redrawShapes()
        {
            Pen blackpen = new Pen(Color.Black);

            for (int i = 0; i < shape.Count; i++)
            {
                shapeIdentify(i, blackpen);
            }

        }
        private void shapeIdentify(int index, Pen pen)
        {
            Type t = shape[index].GetType();
            Graphics g = this.CreateGraphics();

            if (t.Equals(typeof(Square)))
            {
                Square refreshShape = new Square(one, two);
                refreshShape = (Square)shape[index];
                refreshShape.draw(g, pen);

                if (moveByDrag == true && moveByDragIndex == index)
                {
                    differenceX = refreshShape.getXDifference(differenceX);
                    differenceY = refreshShape.getYDifference(differenceY);
                    newPos = refreshShape.setNewPosition(draggedMouseLocation, differenceX, differenceY);
                    refreshShape = new Square(draggedMouseLocation, newPos);
                    shape[shapeIndex] = refreshShape;
                }

                if (rotateBy90 == true && rotationIndex == index)
                {
                    rotatedKeyPt = RotatePt(90, refreshShape.getKeyPt(), refreshShape.getMidPt());
                    rotatedOppPt = RotatePt(90, refreshShape.getOppPt(), refreshShape.getMidPt());
                    refreshShape = new Square(rotatedKeyPt, rotatedOppPt);
                    refreshShape.draw(g, pen);
                    shape[shapeIndex] = refreshShape;
                    rotateBy90 = false;
                }

                if (rotateBy180 == true && rotationIndex == index)
                {
                    rotatedKeyPt = RotatePt(180, refreshShape.getKeyPt(), refreshShape.getMidPt());
                    rotatedOppPt = RotatePt(180, refreshShape.getOppPt(), refreshShape.getMidPt());
                    refreshShape = new Square(rotatedKeyPt, rotatedOppPt);
                    refreshShape.draw(g, pen);
                    shape[shapeIndex] = refreshShape;
                    rotateBy180 = false;
                }
            }


            if (t.Equals(typeof(Triangle)))
            {
                Triangle refreshShape = new Triangle(one, two);
                refreshShape = (Triangle)shape[index];
                refreshShape.draw(g, pen);

                if (moveByDrag == true && moveByDragIndex == index)
                {
                    differenceX = refreshShape.getXDifference(differenceX);
                    differenceY = refreshShape.getYDifference(differenceY);
                    newPos = refreshShape.setNewPosition(draggedMouseLocation, differenceX, differenceY);
                    refreshShape = new Triangle(draggedMouseLocation, newPos);
                    refreshShape.draw(g, pen);
                    shape[shapeIndex] = refreshShape;
                }

                if (rotateBy90 == true && rotationIndex == index)
                {
                    rotatedKeyPt = RotatePt(90, refreshShape.getFirstPt(), refreshShape.getMidPt());
                    rotatedOppPt = RotatePt(90, refreshShape.getTopPt(), refreshShape.getMidPt());
                    refreshShape = new Triangle(rotatedKeyPt, rotatedOppPt);
                    refreshShape.draw(g, pen);
                    shape[shapeIndex] = refreshShape;
                    rotateBy90 = false;
                }
                if (rotateBy180 == true && rotationIndex == index)
                {
                    rotatedKeyPt = RotatePt(180, refreshShape.getFirstPt(), refreshShape.getMidPt());
                    rotatedOppPt = RotatePt(180, refreshShape.getTopPt(), refreshShape.getMidPt());
                    refreshShape = new Triangle(rotatedKeyPt, rotatedOppPt);
                    refreshShape.draw(g, pen);
                    shape[shapeIndex] = refreshShape;
                    rotateBy180 = false;
                }
            }


            if (t.Equals(typeof(Circle)))
            {
                Circle refreshShape = new Circle(one, two);
                refreshShape = (Circle)shape[index];
                refreshShape.draw(g, pen);

                if (moveByDrag == true && moveByDragIndex == index)
                {
                    differenceX = refreshShape.getXDifference(differenceX);
                    differenceY = refreshShape.getYDifference(differenceY);
                    newPos = refreshShape.setNewPosition(draggedMouseLocation, differenceX, differenceY);
                    refreshShape = new Circle(draggedMouseLocation, newPos);
                    refreshShape.draw(g, pen);
                    shape[shapeIndex] = refreshShape;
                }

            }
        }


        // This method is quite important and detects all mouse clicks - other methods may need
        // to be implemented to detect other kinds of event handling eg keyboard presses.
        private void mouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Graphics g = this.CreateGraphics();
                Pen blackpen = new Pen(Color.Black);

                if (selectSquareStatus == true)
                {
                    if (clicknumber == 0)
                    {
                        one = new Point(e.X, e.Y);
                        clicknumber = 1;
                    }
                    else
                    {
                        mouseHeld = true;
                        two = new Point(e.X, e.Y);
                        mouseUp(sender, e);

                        clicknumber = 0;
                        selectSquareStatus = false;

                        Square FinalSquare = new Square(one, two);
                        FinalSquare.draw(g, blackpen);
                        shape.Add(FinalSquare);
                        refreshScreen();

                    }
                }

                if (selectTriangleStatus == true)
                {
                    if (clicknumber == 0)
                    {
                        one = new Point(e.X, e.Y);
                        clicknumber = 1;
                    }
                    else
                    {
                        mouseHeld = true;
                        two = new Point(e.X, e.Y);
                        mouseUp(sender, e);
                        clicknumber = 0;
                        selectTriangleStatus = false;

                        Triangle FinalTriangle = new Triangle(one, two);
                        FinalTriangle.draw(g, blackpen);
                        shape.Add(FinalTriangle);
                        refreshScreen();
                    }
                }

                if (selectCircleStatus == true)
                {
                    if (clicknumber == 0)
                    {
                        one = new Point(e.X, e.Y);
                        clicknumber = 1;
                    }
                    else
                    {
                        mouseHeld = true;
                        two = new Point(e.X, e.Y);
                        mouseUp(sender, e);
                        clicknumber = 0;
                        selectCircleStatus = false;

                        Circle FinalCircle = new Circle(one, two);
                        FinalCircle.draw(g, blackpen);
                        shape.Add(FinalCircle);
                        refreshScreen();
                    }
                }

                if (moveByDrag == true)
                {
                    shapeIdentify(shapeIndex, blackpen);
                    moveByDrag = false;

                }
            }
        }

        private void mouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && clicknumber == 1)
            {
                mouseHeld = true;
            }
        }

        private void mouseMove(object sender, MouseEventArgs e)
        {
            Graphics g = this.CreateGraphics();
            Pen blackpen = new Pen(Color.Red);
            Pen eraser = new Pen(Color.White);
            Square aSquare = new Square(one, draggedMouseLocation);
            Triangle aTriangle = new Triangle(one, draggedMouseLocation);
            Circle aCircle = new Circle(one, draggedMouseLocation);

            if (mouseHeld == true)
            {
                Invalidate();
                Update();
                redrawShapes();

                draggedMouseLocation = e.Location;

                if (selectSquareStatus == true)
                {
                    aSquare.draw(g, blackpen);
                }
                if (selectTriangleStatus == true)
                {
                    aTriangle.draw(g, blackpen);
                }
                if (selectCircleStatus == true)
                {
                    aCircle.draw(g, blackpen);
                }
            }

            if (moveByDrag == true)
            {
                draggedMouseLocation = e.Location;
                moveByDragIndex = shapeIndex;
                shapeIdentify(shapeIndex, blackpen);
                refreshScreen();
            }
        }

        private void mouseUp(object sender, MouseEventArgs e)
        {
            mouseHeld = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }

    abstract class Shape
    {}

    class Square : Shape   // This has Main embedded
    {
        Point keyPt, oppPt;
        Point midPt;
        Point topRightPt;
        Point bottomLeftPt;
        Line line = new Line();

        public Square(Point keyPt, Point oppPt)
        {
            this.keyPt = keyPt;
            this.oppPt = oppPt;
        }

        public void draw(Graphics g, Pen blackPen)
        {
            double xDiff, yDiff, xMid, yMid;

            xDiff = oppPt.X - keyPt.X;
            yDiff = oppPt.Y - keyPt.Y;
            xMid = (oppPt.X + keyPt.X) / 2;
            yMid = (oppPt.Y + keyPt.Y) / 2;
            midPt = new Point((int)xMid, (int)yMid);
            topRightPt = new Point((int)(xMid + yDiff / 2), (int)(yMid - xDiff / 2));
            bottomLeftPt = new Point((int)(xMid - yDiff / 2), (int)(yMid + xDiff / 2));

            line.drawLine(g, keyPt, topRightPt, blackPen);
            line.drawLine(g, oppPt, topRightPt, blackPen);
            line.drawLine(g, bottomLeftPt, oppPt, blackPen);
            line.drawLine(g, bottomLeftPt, keyPt, blackPen);
        }

        public Point getKeyPt()
        {
            return keyPt;
        }

        public Point getOppPt()
        {
            return oppPt;
        }

        public Point getMidPt()
        {
            return midPt;
        }

        public int getXDifference(int xDiff)
        {
            xDiff = oppPt.X - keyPt.X;
            return xDiff;
        }

        public int getYDifference(int yDiff)
        {
            yDiff = oppPt.Y - keyPt.Y;
            return yDiff;

        }

        public Point setNewPosition(Point p, int x, int y)
        {
            Point newPosition = new Point();
            newPosition.X = p.X + x;
            newPosition.Y = p.Y + y;
            return newPosition;
        }
    }

    class Triangle : Shape
    {
        Point firstPt, topPt, midPt, adjPt;

        public Triangle(Point firstPt, Point topPt)
        {
            this.firstPt = firstPt;
            this.topPt = topPt;
        }

        public Point getTopPt()
        {
            return topPt;
        }

        public Point getMidPt()
        {
            return midPt;
        }

        public Point getFirstPt()
        {
            return firstPt;
        }

        public int getXDifference(int xDiff)
        {
            xDiff = topPt.X - firstPt.X;
            return xDiff;
        }

        public int getYDifference(int yDiff)
        {
            yDiff = firstPt.Y - topPt.Y;
            return yDiff;
        }

        public Point setNewPosition(Point p, int x, int y)
        {
            Point newPosition = new Point();
            newPosition.X = p.X + x;
            newPosition.Y = p.Y - y;
            return newPosition;
        }

        public void draw(Graphics g, Pen blackPen)
        {
            Point point2;
            point2 = new Point(0, 0);
            double difference;

            if (firstPt.Y > topPt.Y)
            {
                point2.Y = firstPt.Y;
            }
            else
            {
                point2.Y = topPt.Y;
            }

            if (firstPt.X > topPt.X)
            {
                difference = firstPt.X - topPt.X;
                point2.X = firstPt.X + (int)difference;
            }
            else
            {
                difference = topPt.X - firstPt.X;
                point2.X = topPt.X + (int)difference;
            }

            adjPt.X = topPt.X + (int)difference;
            adjPt.Y = firstPt.Y;
            midPt.X = topPt.X;
            midPt.Y = (firstPt.Y + topPt.Y) / 2;

            Line line = new Line();
            line.drawLine(g, firstPt, topPt, blackPen);
            line.drawLine(g, topPt, adjPt, blackPen);
            line.drawLine(g, firstPt, adjPt, blackPen);
        }
    }

    class Circle : Shape
    {
        Point keyPt, oppPt;
        Point[] points = new Point[4];
        Pen blackPen = new Pen(Color.Black, 1);

        public Circle(Point keyPt, Point oppPt)
        {
            this.keyPt = keyPt;
            this.oppPt = oppPt;
        }
        public void draw(Graphics g, Pen p)
        {
            CurveLine curve = new CurveLine();
            curve.drawCircle(g, keyPt, oppPt, p);
        }

        public int getXDifference(int xDiff)
        {
            xDiff = oppPt.X - keyPt.X;
            return xDiff;
        }
        public int getYDifference(int yDiff)
        {
            yDiff = keyPt.Y - oppPt.Y;
            return yDiff;

        }
        public Point setNewPosition(Point p, int x, int y)
        {
            Point newPosition = new Point();
            newPosition.X = p.X + x;
            newPosition.Y = p.Y - y;
            return newPosition;
        }
    }

    class Line : Shape
    {
        int xDiff, yDiff;
        int steps;
        float xInc, yInc, x, y;
        Point KeyPt, OppPt, MidPt;

        public Line()
        {
        }

        public void drawLine(Graphics g, Point startPt, Point endPt, Pen pen)
        {
            KeyPt = startPt;
            OppPt = endPt;

            if (startPt.X > endPt.X)
            {
                xDiff = startPt.X - endPt.X;
                x = endPt.X;


            }
            else
            {
                xDiff = endPt.X - startPt.X;
                x = startPt.X;
            }

            if (startPt.Y > endPt.Y)
            {
                yDiff = startPt.Y - endPt.Y;
                y = endPt.Y;
            }
            else
            {
                yDiff = endPt.Y - startPt.Y;
                y = startPt.Y;
            }
            if (startPt.Y == endPt.Y)
            {
                y = startPt.Y;
            }

            MidPt.X = (int)((xDiff / 2) + x);
            MidPt.Y = (int)((yDiff / 2) + y);

            if (Math.Abs(xDiff) > Math.Abs(yDiff))
            {
                steps = Math.Abs(xDiff);
            }
            else
            {
                steps = Math.Abs(yDiff);
            }

            xInc = xDiff / (float)steps;
            yInc = yDiff / (float)steps;

            if (startPt.X > endPt.X && startPt.Y < endPt.Y)
            {
                x = endPt.X;
                y = endPt.Y;
                yInc = -yInc;
            }

            if (startPt.X < endPt.X && startPt.Y > endPt.Y)
            {
                x = startPt.X;
                y = startPt.Y;
                yInc = -yInc;
            }

            if (startPt.X > endPt.X && startPt.Y > endPt.Y)
            {
                x = endPt.X;
                y = endPt.Y;
            }

            if (startPt.X < endPt.X && startPt.Y < endPt.Y)
            {
                x = startPt.X;
                y = startPt.Y;
            }

            for (int i = 0; i < steps; i++)
            {
                x += xInc;
                y += yInc;
                putPixel(g, (int)Math.Round(x), (int)Math.Round(y), pen);
            }
        }

        public void putPixel(Graphics g, int x, int y, Pen p)
        {
            System.Drawing.SolidBrush brush = new System.Drawing.SolidBrush(p.Color);
            g.FillRectangle(brush, x, y, 1, 1);
        }
    }

    class CurveLine : Shape
    {
        public void drawCircle(Graphics g, Point center, Point oppPt, Pen p)
        {
            int x, y;
            int radius = center.Y - oppPt.Y;
            int decision = 3 - 2 * radius;
            y = radius;
            x = 0;

            draw(g, center.X, center.Y, x, y, p);

            while (y >= x)
            {
                x++;

                if (decision > 0)
                {
                    y--;
                    decision = decision + 4 * (x - y) + 10;
                }
                else
                    decision = decision + 4 * x + 6;
                draw(g, center.X, center.Y, x, y, p);
            }
        }

        public void draw(Graphics g, int xCentre, int yCentre, int x, int y, Pen p)
        {
            putPixel(g, xCentre + x, yCentre + y, p);
            putPixel(g, xCentre - x, yCentre + y, p);
            putPixel(g, xCentre + x, yCentre - y, p);
            putPixel(g, xCentre - x, yCentre - y, p);
            putPixel(g, xCentre + y, yCentre + x, p);
            putPixel(g, xCentre - y, yCentre + x, p);
            putPixel(g, xCentre + y, yCentre - x, p);
            putPixel(g, xCentre - y, yCentre - x, p);
        }
        public void putPixel(Graphics g, int x, int y, Pen p)
        {
            Pen pen = new Pen(Color.Red);
            System.Drawing.SolidBrush brush = new System.Drawing.SolidBrush(p.Color);
            g.FillRectangle(brush, x, y, 1, 1);
        }
    }
}