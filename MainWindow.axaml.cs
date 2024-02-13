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
	protected double x;
	protected double y;
	static Shape() { }

    abstract public void Draw(Canvas canv);
	public virtual bool IsInside(double x, double y) { if (this.x == x && this.y == y) return true; else return false; }

	public double X
	{
		get { return x; }
		set { x = value; }
	}
	public double Y
	{
		get { return x; }
		set { x = value; }
	}
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
		{ Width = Radius, Height = Radius, Fill = Avalonia.Media.Brushes.DarkRed, StrokeThickness = 1, Stroke = Avalonia.Media.Brushes.Black };
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
	private double z = Radius / 4 * Math.Pow(3, 0.5);
	public Triangle(double x, double y)
	{
		this.x = x;
		this.y = y;

	}
	public override void Draw(Canvas canv)
	{
		
		List<Avalonia.Point> tmp = new List<Avalonia.Point>() { new Avalonia.Point(this.x - z, this.y + Radius / 4), new Avalonia.Point(this.x + z, this.y + Radius / 4), new Avalonia.Point(this.x, this.y - Radius/2) };
		Polygon shape = new Polygon() { Points = tmp, Fill = Avalonia.Media.Brushes.DarkRed, StrokeThickness = 1, Stroke = Avalonia.Media.Brushes.Black };
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
		{ Width = Radius, Height = Radius, Fill = Avalonia.Media.Brushes.DarkRed, StrokeThickness = 1, Stroke = Avalonia.Media.Brushes.Black };
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
	private string type = "square";
	public MainWindow()
	{
		InitializeComponent();
		AddButtons();
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
		double X = e.GetCurrentPoint(canv).Position.X;
		double Y = e.GetCurrentPoint(canv).Position.Y;
		foreach (Shape shape in shapes)
		{
			if (shape.IsInside(X, Y))
			{
				return;
			}

		}
		if(type == "circle")
		{
			shapes.Add(new Circle(X, Y));
		}
		else if(type == "square")
		{
			shapes.Add(new Square(X, Y));
		}
		else if (type == "triangle")
		{
			shapes.Add(new Triangle(X, Y));
		}

		Redraw();
	}
	private void PointerReleased(object sender, PointerReleasedEventArgs e)
	{

	}
	private Button CreateBtn(int w, string content, string name, EventHandler<RoutedEventArgs> x)
	{
		Button b = new Button();
		b.Width = w;
		b.Height = 30;
		b.Content = content;
		b.Name = name;
		b.Click += x;
		b.Background = Avalonia.Media.Brushes.Gray;
		b.BorderBrush = Avalonia.Media.Brushes.Black;
		return b;
	}
	private void AddButtons()
	{
		Button b_square =CreateBtn(65, "square", "switch_to_square", button_click_square);
		Button b_circle = CreateBtn(57, "circle", "switch_to_circle", button_click_circle);
		Button b_triangle = CreateBtn(72, "triangle", "switch_to_triangle", button_click_triangle);
		Buttons.Children.Add(b_square);
		Buttons.Children.Add(b_circle);
		Buttons.Children.Add(b_triangle);

		
	}

	private void button_click_circle(object sender, RoutedEventArgs args)
	{
		type = "circle";
	}
	private void button_click_square(object sender, RoutedEventArgs args)
	{
		type = "square";
	}
	private void button_click_triangle(object sender, RoutedEventArgs args)
	{
		type = "triangle";
	}
}
