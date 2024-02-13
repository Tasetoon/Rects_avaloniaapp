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
	public static double Radius = 100;
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
		{ Width = Radius, Height = Radius, Fill = Avalonia.Media.Brushes.DarkRed, StrokeThickness = 5, Stroke = Avalonia.Media.Brushes.Black };
		canv.Children.Add(shape);
		Canvas.SetLeft(shape, x - Radius / 2);
		Canvas.SetTop(shape, y - Radius / 2);

	}
	public override bool IsInside(double x, double y)
	{
		if (Math.Abs(x - this.x) <= Radius / 2 && Math.Abs(y - this.y) <= Radius / 2) { return true; } else { return false; }
	}

}
class Triangle : Shape
{
	private double z = Radius / 2 * Math.Pow(3, 0.5);
	public Triangle(double x, double y)
	{
		this.x = x;
		this.y = y;

	}
	public override void Draw(Canvas canv)
	{

		List<Avalonia.Point> tmp = new List<Avalonia.Point>() { new Avalonia.Point(this.x - z, this.y + Radius / 2), new Avalonia.Point(this.x + z, this.y + Radius / 2), new Avalonia.Point(this.x, this.y - Radius) };
		Polygon shape = new Polygon() { Points = tmp, Fill = Avalonia.Media.Brushes.DarkRed, StrokeThickness = 5, Stroke = Avalonia.Media.Brushes.Black };
		canv.Children.Add(shape);
		Canvas.SetLeft(shape, 0);
		Canvas.SetTop(shape, 0);

	}
	public override bool IsInside(double x, double y)
	{
		double z = Math.Pow(3, 0.5);

		if (y <= (this.y + (Radius / 2)) && y >= (x - this.x) * z + this.y - Radius && y >= -1 * (x - this.x) * z + this.y - Radius) { return true; } else { return false; }
	}

}
class Circle : Shape
{
	public Circle(double x, double y)
	{
		this.x = x;
		this.y = y;

	}
	public override void Draw(Canvas canv)
	{
		Ellipse shape = new Ellipse()
		{ Width = Radius, Height = Radius, Fill = Avalonia.Media.Brushes.DarkRed, StrokeThickness = 5, Stroke = Avalonia.Media.Brushes.Black };
		canv.Children.Add(shape);
		Canvas.SetLeft(shape, x - Radius / 2);
		Canvas.SetTop(shape, y - Radius / 2);
	}
	public override bool IsInside(double x, double y) //работает
	{
		if (Math.Pow(x - this.x, 2) + Math.Pow(y - this.y, 2) <= Math.Pow(Radius, 2) / 4) { return true; } else { return false; }
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
		shapes.Add(new Triangle(X, Y));
		Redraw();
	}


}
