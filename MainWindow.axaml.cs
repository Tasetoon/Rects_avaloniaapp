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
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Globalization;

namespace Rects;

abstract class Shape
{
	protected double Radius = 50;
	protected double x;
	protected double y;
	protected bool IsSelected = false;
	protected double Dx;
	protected double Dy;
	protected string type;
	protected Avalonia.Media.IBrush color;

	static Shape() { }

	abstract public void Draw(Canvas canv);
	public virtual bool IsInside(double x, double y) { if (this.x == x && this.y == y) return true; else return false; }

	public double R
	{
		get { return Radius; }
		set { Radius = value; }
	}
	public string Type
	{
		get { return type; }
	}
	public void SetPoint(double x, double y)
	{
		this.x = x;
		this.y = y;
	}

	public bool IsSELECTED
	{
		get { return IsSelected; }
		set { IsSelected = value; }
	}
	public double DX
	{
		get { return Dx; }
		set { Dx = value; }
	}
	public double DY
	{
		get { return Dy; }
		set { Dy = value; }
	}
	public double X
	{
		get { return x; }
		set { x = value; }
	}
	public double Y
	{
		get { return y; }
		set { y = value; }
	}
	public IBrush Color
	{
		get { return color; }
		set { color = value; }
	}
	
}
class Square : Shape
{
	
	public Square(double x, double y)
	{
		this.x = x;
		this.y = y;
		type = "square";

	}
	public override void Draw(Canvas canv)
	{
		Avalonia.Controls.Shapes.Rectangle shape = new Avalonia.Controls.Shapes.Rectangle()
		{ Width = R, Height = R, StrokeThickness = 1, Stroke = Avalonia.Media.Brushes.Black, Fill = color};
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
	public Triangle(double x, double y)
	{
		this.x = x;
		this.y = y;
		type = "triangle";
	}
	public override void Draw(Canvas canv)
	{

		double z = R / 4 * Math.Pow(3, 0.5);
		List<Avalonia.Point> tmp = new List<Avalonia.Point>() { new Avalonia.Point(this.x - z, this.y + Radius / 4), new Avalonia.Point(this.x + z, this.y + Radius / 4), new Avalonia.Point(this.x, this.y - Radius / 2) };
		Polygon shape = new Polygon() { Points = tmp, Fill = color, StrokeThickness = 1, Stroke = Avalonia.Media.Brushes.Black };
		canv.Children.Add(shape);
		Canvas.SetLeft(shape, 0);
		Canvas.SetTop(shape, 0);


	}
	public override bool IsInside(double x, double y)
	{
		double z = Math.Pow(3, 0.5);

		if (y <= (this.y + (Radius / 4)) && y >= (x - this.x) * z + this.y - Radius/2 && y >= -1 * (x - this.x) * z + this.y - Radius/2) { return true; } else { return false; }
	}

}
class Circle : Shape
{
	public Circle(double x, double y)
	{
		this.x = x;
		this.y = y;
		type = "circle";
	}
	public override void Draw(Canvas canv)
	{
		Ellipse shape = new Ellipse()
		{ Width = this.R, Height = this.R, StrokeThickness = 1, Stroke = Avalonia.Media.Brushes.Black, Fill = color};
		canv.Children.Add(shape);
		Canvas.SetLeft(shape, x - Radius / 2);
		Canvas.SetTop(shape, y - Radius / 2);
	}
	public override bool IsInside(double x, double y)
	{
		if (Math.Pow(x - this.x, 2) + Math.Pow(y - this.y, 2) <= Math.Pow(Radius, 2) / 4) { return true; } else { return false; }
	}

}
public partial class MainWindow : Window
{
	
	List<Shape> shapes = new List<Shape>();
	private string type = "square";
	protected bool IsAnySelected = false;
	private Random random = new Random();

	private int content_radius = 50;
	
	public MainWindow()
	{
		InitializeComponent();
		Navbar();
	}
	private void Redraw()
	{

		canv.Children.Clear();
		for(int i = 0; i <  shapes.Count; i += 3)
		{
			shapes[i].Color = Avalonia.Media.Brushes.White;
			
		}
		for (int i = 1; i < shapes.Count; i += 3)
		{
			shapes[i].Color = Avalonia.Media.Brushes.Blue;

		}
		for (int i = 2; i < shapes.Count; i += 3)
		{
			shapes[i].Color = Avalonia.Media.Brushes.Red;

		}
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
				shape.IsSELECTED = true;
				IsAnySelected = true;
				shape.DX = X - shape.X;
				shape.DY = Y - shape.Y;
			}

		}
		if(!IsAnySelected)
		{
			//here we control the correct spawning
			foreach (Shape shape in shapes)
			{
				if (Y <= 25 || Y >= this.Bounds.Bottom - shape.R || X - shape.R/2 <= 0 || X + shape.R/2 >= this.Bounds.Width)
				{
					return;
				}

			}
			
			
			if (type == "circle")
			{
				shapes.Add(new Circle(X, Y));
			}
			else if (type == "square")
			{
				shapes.Add(new Square(X, Y));
			}
			else if (type == "triangle")
			{
				shapes.Add(new Triangle(X, Y));
			}
			
			
		}
		

		Redraw();
	}
	private void PMoved(object sender, PointerEventArgs e)
	{
		if(!IsAnySelected)
		{
			return;
		}
		foreach(Shape shape in shapes)
		{
			if (shape.IsSELECTED)
			{
				double X = e.GetCurrentPoint(canv).Position.X;
				double Y = e.GetCurrentPoint(canv).Position.Y;

				shape.SetPoint(X-shape.DX, Y-shape.DY);

				if(shape.X + shape.R/2 >= this.Bounds.Width || shape.X - shape.R / 2 <= 0)
				{
					shape.IsSELECTED = false;
					return;
				}

				if(shape.Type == "triangle")
				{
					if (shape.Y + shape.R*0.84 >= this.Bounds.Bottom)
					{
						shape.IsSELECTED = false;
						return;
					}
				}
				else if (shape.Y>= this.Bounds.Bottom - shape.R)
				{
					shape.IsSELECTED = false;
					return;
				}
				if (shape.Y-shape.R/2 <= 0)
				{
					shape.IsSELECTED = false;
					return;
				}
			}
			
		}
		Redraw();
	}
	private void PReleased(object sender, PointerReleasedEventArgs e)
	{
		IsAnySelected = false;
		foreach (Shape shape in shapes)
		{
			shape.IsSELECTED = false;
		}
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
	private void Navbar()
	{
		Button b_square =CreateBtn(65, "square", "switch_to_square", button_click_square);
		Button b_circle = CreateBtn(57, "circle", "switch_to_circle", button_click_circle);
		Button b_triangle = CreateBtn(72, "triangle", "switch_to_triangle", button_click_triangle);
		Button b_clear = CreateBtn(50, "clear", "clear_all_shapes", button_click_clear);
		Button b_random = CreateBtn(70, "random", "random", button_click_random);
		
		/*Line line = new Line();
		line.StartPoint = new Avalonia.Point(0, 10);
		line.EndPoint = new Avalonia.Point(100, 10);
		line.Stroke = Avalonia.Media.Brushes.Black;
		line.StrokeThickness = 2;*/


		Buttons.Children.Add(b_square);
		Buttons.Children.Add(b_circle);
		Buttons.Children.Add(b_triangle);
		Buttons.Children.Add(b_clear);
		Buttons.Children.Add(b_random);
		//Line.Children.Add(line);	
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
	private void button_click_clear(object sender, RoutedEventArgs args)
	{
		shapes.Clear();
		this.content_radius = 50;
		r_indicator.Text = $"radius: {this.content_radius}";
		Redraw();
	}
	private void button_click_random(object sender, RoutedEventArgs args)
	{
		foreach (var shape in shapes)
		{
			shape.SetPoint(random.Next((int)shape.R/2, (int)(this.Bounds.Width-shape.R/2)), random.Next((int)shape.R/2, (int)(this.Bounds.Bottom - shape.R)));
			
		}
		Redraw();
	}
	

	private void KeyDown(object sender, KeyEventArgs e)
	{
		switch (e.Key)
		{
			case Key.OemMinus:
				r_indicator.Text = $"radius: {content_radius -= 5}";
				foreach (var shape in shapes)
				{

					shape.R -= 5;
				}
				Redraw();
				break;
			case Key.OemPlus:
				r_indicator.Text = $"radius: {content_radius += 5}";
				foreach (var shape in shapes)
				{
					shape.R += 5;
				}
				Redraw();
				break;
			
			/*case Key.D:
				if (rectangles.Count == 0) break;
				else
				{
					canv.Children.Remove(rectangles[rectangles.Count - 1].R);
					rectangles.RemoveAt(rectangles.Count - 1);
				}
				break;*/
		}
	}
}
