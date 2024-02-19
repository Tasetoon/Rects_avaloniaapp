using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using System.Collections.Generic;
using System;

namespace Rects;

abstract class Shape
{
	protected double Radius = 50;
	protected double x;
	protected double y;
	protected bool IsSelected = false;
	protected double Dx;
	protected double Dy;
	protected string? type;
	protected IBrush? color;

	static Shape() { }

	abstract public void Draw(Canvas canv);
	public virtual bool IsInside(double x, double y) { if (this.x == x && this.y == y) return true; else return false; }

	public double R
	{
		get { return Radius; }
		set { Radius = value; }
	}
	public string Type => type;
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
		Rectangle shape = new()
		{ Width = R, Height = R, StrokeThickness = 1, Stroke = Brushes.Black, Fill = color};
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
		List<Avalonia.Point> tmp = new() { new Avalonia.Point(x - z, y + Radius / 4), new Avalonia.Point(x + z, y + Radius / 4), new Avalonia.Point(x, y - Radius / 2) };
		Polygon shape = new() { Points = tmp, Fill = color, StrokeThickness = 1, Stroke = Brushes.Black };
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
		Ellipse shape = new()
		{ Width = R, Height = R, StrokeThickness = 1, Stroke = Brushes.Black, Fill = color};
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
	private readonly Random random = new();


	readonly List<Shape> shapes = new();
	readonly List<int> cur_radiuses = new();

	protected bool IsAnySelected = false;

	private string type = "square";




	public MainWindow()
	{
		InitializeComponent();
		Navbar();
	}
	private void UpdateRadiusIndicator()
	{
		HashSet<int> SetOfRadiuses = new();
		cur_radiuses.Sort();
		foreach (int x in cur_radiuses)
		{
			SetOfRadiuses.Add(x);
		}

		r_indicator.Text = $"radiuses: {string.Join(",", SetOfRadiuses)}";
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
			
			if (Y <= 25 || Y >= this.Bounds.Bottom - 25 || X - 25 <= 0 || X + 25 >= this.Bounds.Width)
			{
				return;
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
			cur_radiuses.Add(50);
			UpdateRadiusIndicator();
		}
		

		Redraw();
	}
	private void PMoved(object sender, PointerEventArgs e)
	{
		if(!IsAnySelected)
		{
			return;
		}
		//here we`re trying to control touching the bounds of the window
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
	private static Button CreateBtn(int w, string content, string name, EventHandler<RoutedEventArgs> x)
	{
		Button b = new()
		{
			Width = w,
			Height = 30,
			Content = content,
			Name = name,
			Background = Avalonia.Media.Brushes.Gray,
			BorderBrush = Avalonia.Media.Brushes.Black

		};
		
		b.Click += x;
		return b;
	}
	private void Navbar()
	{
		List<Button> btns = new()
		{
			CreateBtn(65, "square", "switch_to_square", Button_click_square),
			CreateBtn(57, "circle", "switch_to_circle", Button_click_circle),
			CreateBtn(72, "triangle", "switch_to_triangle", Button_click_triangle),
			CreateBtn(50, "clear", "clear_all_shapes", Button_click_clear),
			CreateBtn(70, "random", "random", Button_click_random)
		};

		foreach (Button b in btns)
		{
			Buttons.Children.Add(b);
		}
	}

	private void Button_click_circle(object sender, RoutedEventArgs args)
	{
		type = "circle";
	}
	private void Button_click_square(object sender, RoutedEventArgs args)
	{
		type = "square";
	}
	private void Button_click_triangle(object sender, RoutedEventArgs args)
	{
		type = "triangle";
	}
	private void Button_click_clear(object sender, RoutedEventArgs args)
	{
		shapes.Clear();
		cur_radiuses.Clear();
		r_indicator.Text = $"radius: 50";
		Redraw();
	}
	private void Button_click_random(object sender, RoutedEventArgs args)
	{
		foreach (var shape in shapes)
		{
			shape.SetPoint(random.Next((int)shape.R/2, (int)(this.Bounds.Width-shape.R/2)), random.Next((int)shape.R/2, (int)(this.Bounds.Bottom - shape.R)));
			
		}
		Redraw();
	}
	

	private new void KeyDown(object sender, KeyEventArgs e)
	{
		switch (e.Key)
		{
			case Key.OemMinus:
				foreach (var shape in shapes)
				{
					shape.R -= 5;
				}
				for(int i = 0; i < cur_radiuses.Count; ++i)
				{
					cur_radiuses[i] -= 5;
				}
				UpdateRadiusIndicator();
				Redraw();
				break;
			case Key.OemPlus:
				foreach (var shape in shapes)
				{
					shape.R += 5;
				}
				for (int i = 0; i < cur_radiuses.Count; ++i)
				{
					cur_radiuses[i] += 5;
				}
				UpdateRadiusIndicator();
				Redraw();
				break;
			
			case Key.Z:
				if (shapes.Count == 0) return;
				for (int i = 0; i < cur_radiuses.Count; ++i)
				{

					if (cur_radiuses[i] == shapes[^1].R) // ^1 means the last index of collection
					{
						cur_radiuses.RemoveAt(i);
						UpdateRadiusIndicator();
						break;
					}

				}
				shapes.RemoveAt(shapes.Count - 1);
				if(shapes.Count == 0)
				{
					cur_radiuses.Clear();
					cur_radiuses.Add(50);
					r_indicator.Text = $"radius: 50";
				}
				
				Redraw();
				break;
				
				
		}
	}
}
