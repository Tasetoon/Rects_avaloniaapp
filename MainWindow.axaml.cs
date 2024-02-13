using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using System.Collections.Generic;
using System;
using System.Drawing;
using System.Net;
using Avalonia;

namespace Rects;

abstract class Shape
{
	public static double Radius = 50;
	public static string Color;
	protected double x;
	protected double y;
	static Shape() { }

    abstract public void Draw(Canvas canv);
	public virtual bool IsInside(double x, double y) { if (this.x == x && this.y == y) return true; else return false; }
}
class Square : Shape
{
	public Square(double x, double y)
	{
		this.x = x;
		this.y = y;

	}
	public override void Draw(Canvas canv)
    {
        Avalonia.Controls.Shapes.Rectangle shape = new Avalonia.Controls.Shapes.Rectangle()
        { Width = Radius, Height = Radius, Fill = Avalonia.Media.Brushes.Aqua };
        canv.Children.Add(shape);
        Canvas.SetLeft(shape, x);
        Canvas.SetTop(shape, y);

    }
	public override bool IsInside(double x, double y)
	{
		if (Math.Abs(x - this.x) <= Radius / 2 && Math.Abs(y - this.y) <= Radius / 2) { return true; } else { return false; }
	}

}
class Triangle : Shape
{
	public Triangle(double x, double y)
	{
		this.x = x;
		this.y = y;

	}
	public override void Draw(Canvas canv)
	{

		
	}
	public override bool IsInside(double x, double y) //работает
	{
		if (y <= this.x - x * Math.Pow(3, 0.5) && y <= this.x - x * (-1) * Math.Pow(3, 0.5) && y >= this.y - (Radius / 2)) { return true; } else { return false; }
	}

}
class Circle : Shape
{
	public Circle(double x, double y)
	{
		this.x = x;
		this.y = y;

	}
	public override void Draw(Canvas canv) {
		Avalonia.Controls.Shapes.Ellipse shape = new Avalonia.Controls.Shapes.Ellipse()
		{ Width = Radius, Height = Radius, Fill = Avalonia.Media.Brushes.Aqua };
		canv.Children.Add(shape);
		Canvas.SetLeft(shape, x);
		Canvas.SetTop(shape, y);
	}
	public override bool IsInside(double x, double y) //работает
	{
		if (Math.Pow(x - this.x, 2) + Math.Pow(y - this.y, 2) <= Math.Pow(Radius, 2)) { return true; } else { return false; }
	}

}
public partial class MainWindow : Window
{
	List<Shape> shapes = new List<Shape>();
	public MainWindow()
	{
		InitializeComponent();
	}
	public void Redraw()
	{
		canv.Children.Clear();
		foreach (Shape shape in shapes)
		{
			shape.Draw(canv);
		}
	}
	private void PPressed(object sender, PointerPressedEventArgs e)
	{
		int X = (int)e.GetCurrentPoint(canv).Position.X;
		int Y = (int)e.GetCurrentPoint(canv).Position.Y;
		foreach (Shape shape in shapes)
		{
			if (shape.IsInside(X, Y)) return;
		}
		shapes.Add(new Circle(X - Shape.Radius/2, Y - Shape.Radius / 2));
		Redraw();
	}
	/*private void PWheel(object sender, PointerWheelEventArgs e)
	{
		//Console.WriteLine($"Pointer pressed: {e.KeyModifiers}, {e.Delta}, {e.GetCurrentPoint(null).Position}");
		if (e.Delta.Y == 1) speed++;
		else speed--;
		if (speed < 6) { speed = 5; }
		lb.Content = $"Speed: {speed}";
	}*/
	/*private void Move(string Direction)
	{
		if (Direction == "Up")
		{

			for (int i = 0; i < rectangles.Count; ++i)
			{
				Canvas.SetLeft(rectangles[i].R, rectangles[i].X);
				Canvas.SetTop(rectangles[i].R, rectangles[i].Y - speed);
				rectangles[i].Y -= speed;

			}
		}
		else if (Direction == "Down")
		{
			for (int i = 0; i < rectangles.Count; ++i)
			{
				Canvas.SetLeft(rectangles[i].R, rectangles[i].X);
				Canvas.SetTop(rectangles[i].R, rectangles[i].Y + speed);
				rectangles[i].Y += speed;

			}
		}
		else if (Direction == "Left")
		{
			for (int i = 0; i < rectangles.Count; ++i)
			{

				Canvas.SetLeft(rectangles[i].R, rectangles[i].X - speed);
				Canvas.SetTop(rectangles[i].R, rectangles[i].Y);
				rectangles[i].X -= speed;
			}
		}
		else if (Direction == "Right")
		{
			for (int i = 0; i < rectangles.Count; ++i)
			{

				Canvas.SetLeft(rectangles[i].R, rectangles[i].X + speed);
				Canvas.SetTop(rectangles[i].R, rectangles[i].Y);
				rectangles[i].X += speed;
			}
		}


	}*/

	/*private void KeyDown(object sender, KeyEventArgs e)
	{
		switch (e.Key)
		{
			case Key.Up:
				Move("Up");
				break;
			case Key.Down:
				Move("Down");
				break;
			case Key.Left:
				Move("Left");
				break;
			case Key.Right:
				Move("Right");
				break;
			case Key.D:
				if (rectangles.Count == 0) break;
				else
				{
					canv.Children.Remove(rectangles[rectangles.Count - 1].R);
					rectangles.RemoveAt(rectangles.Count - 1);
				}
				break;
		}
	}*/
	/*private MyRectangle CreateRect()
	{
		Avalonia.Controls.Shapes.Rectangle rect = new Avalonia.Controls.Shapes.Rectangle();
		rect.Width = 20;
		rect.Height = 20;
		rect.Fill = Avalonia.Media.Brushes.MediumPurple;
		canv.Children.Add(rect);
		MyRectangle r = new MyRectangle(rect);
		rectangles.Add(r);
		return r;
	}*/


}
