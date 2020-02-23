using UnityEngine;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;

public class TagText
{
	public class Parameter
	{
		public string Name;
		public string Value;

		public Color GetColor()
		{
			Color value;
			ColorUtility.TryParseHtmlString(Value, out value);
			return value;
		}

		public bool GetBool()
		{
			bool value;
			bool.TryParse(Value, out value);
			return value;
		}

		public int GetInteger()
		{
			int value;
			int.TryParse(Value, out value);
			return value;
		}

		public float GetFloat()
		{
			float value;
			float.TryParse(Value, out value);
			return value;
		}

		public string GetString()
		{
			return Value;
		}
	}

	public class Node
	{
		public int             Index;
		public int             Length;
		public string          Tag;
		public List<Parameter> Parameters;

		public Node(string _Tag)
		{
			Tag        = _Tag;
			Parameters = new List<Parameter>();
		}

		public Parameter GetParameter(string _Name)
		{
			return Parameters.FirstOrDefault(_Parameter => _Parameter.Name == _Name);
		}

		public Color GetColor(string _Name)
		{
			Parameter parameter = GetParameter(_Name);
			return parameter != null ? parameter.GetColor() : Color.white;
		}

		public Color GetColor(string _Name, Color _Default)
		{
			Parameter parameter = GetParameter(_Name);
			return parameter != null ? parameter.GetColor() : _Default;
		}

		public bool GetBool(string _Name, bool _Default = false)
		{
			Parameter parameter = GetParameter(_Name);
			return parameter != null ? parameter.GetBool() : _Default;
		}

		public int GetInteger(string _Name, int _Default = 0)
		{
			Parameter parameter = GetParameter(_Name);
			return parameter != null ? parameter.GetInteger() : _Default;
		}

		public float GetFloat(string _Name, float _Default = 0)
		{
			Parameter parameter = GetParameter(_Name);
			return parameter != null ? parameter.GetFloat() : _Default;
		}

		public string GetString(string _Name, string _Default = null)
		{
			Parameter parameter = GetParameter(_Name);
			return parameter != null ? parameter.GetString() : _Default;
		}
	}

	enum State
	{
		None,
		Tag,
		Open,
		Close,
		ParameterName,
		ParameterValue,
	}

	public string Text
	{
		get { return m_Text; }
	}

	public List<Node> Nodes
	{
		get { return m_Nodes; }
	}

	string     m_Text;
	List<Node> m_Nodes;

	public void Load(string _Text)
	{
		if (m_Nodes == null)
			m_Nodes = new List<Node>();
		else
			m_Nodes.Clear();
		
		m_Text = _Text;
		
		if (string.IsNullOrEmpty(_Text))
			return;
		
		State state = State.None;
		
		Stack<Node> opened = new Stack<Node>();
		
		StringBuilder rawBuilder        = new StringBuilder();
		StringBuilder tagBuilder        = new StringBuilder();
		StringBuilder textBuilder       = new StringBuilder();
		StringBuilder parametersBuilder = new StringBuilder();
		
		using (StringReader reader = new StringReader(_Text))
		{
			while (reader.Peek() >= 0)
			{
				char character = (char)reader.Read();
				
				switch (state)
				{
					case State.None:
						if (character == '<')
						{
							state = State.Tag;
							
							rawBuilder.Append(character);
						}
						else
						{
							textBuilder.Append(character);
						}
						break;
					
					case State.Tag:
						rawBuilder.Append(character);
						if (character != '/')
						{
							state = State.Open;
							
							tagBuilder.Append(character);
						}
						else
						{
							state = State.Close;
						}
						break;
					
					case State.Open:
						rawBuilder.Append(character);
						if (character == '>')
						{
							state = State.None;
							
							Node node = new Node(tagBuilder.ToString());
							node.Index = textBuilder.Length;
							
							opened.Push(node);
							
							rawBuilder.Clear();
							tagBuilder.Clear();
						}
						else if (character == ' ')
						{
							state = State.ParameterName;
							
							Node node = new Node(tagBuilder.ToString());
							node.Index = textBuilder.Length;
							
							opened.Push(node);
							
							tagBuilder.Clear();
						}
						else
						{
							tagBuilder.Append(character);
						}
						break;
					
					case State.Close:
						rawBuilder.Append(character);
						if (character == '>')
						{
							state = State.None;
							
							string tag = tagBuilder.ToString();
							tagBuilder.Clear();
							
							while (opened.Count > 0)
							{
								Node node = opened.Pop();
								
								if (node.Tag == tag)
								{
									node.Length = textBuilder.Length - node.Index;
									
									m_Nodes.Add(node);
									
									rawBuilder.Clear();
									
									break;
								}
								
								textBuilder.Insert(node.Index, $"<{node.Tag}>");
							}
							
							textBuilder.Append(rawBuilder);
							rawBuilder.Clear();
						}
						else
						{
							tagBuilder.Append(character);
						}
						break;
					
					case State.ParameterName:
						rawBuilder.Append(character);
						if (character == '=')
						{
							state = State.ParameterValue;
							
							Parameter parameter = new Parameter();
							parameter.Name = parametersBuilder.ToString();
							parametersBuilder.Clear();
							
							Node node = opened.Peek();
							node.Parameters.Add(parameter);
						}
						else if (character == ' ')
						{
							state = State.ParameterName;
							
							Parameter parameter = new Parameter();
							parameter.Name = parametersBuilder.ToString();
							
							Node node = opened.Peek();
							node.Parameters.Add(parameter);
							
							parametersBuilder.Clear();
						}
						else if (character == '>')
						{
							state = State.None;
							
							Parameter parameter = new Parameter();
							parameter.Name = parametersBuilder.ToString();
							parametersBuilder.Clear();
							
							Node node = opened.Peek();
							node.Parameters.Add(parameter);
							
							rawBuilder.Clear();
						}
						else
						{
							parametersBuilder.Append(character);
						}
						break;
					
					case State.ParameterValue:
						rawBuilder.Append(character);
						if (character == ' ')
						{
							state = State.ParameterName;
							
							Node node = opened.Peek();
							
							Parameter parameter = node.Parameters[node.Parameters.Count - 1];
							parameter.Value = parametersBuilder.ToString();
							
							parametersBuilder.Clear();
						}
						else if (character == '>')
						{
							state = State.None;
							
							Node node = opened.Peek();
							
							Parameter parameter = node.Parameters[node.Parameters.Count - 1];
							parameter.Value = parametersBuilder.ToString();
							
							parametersBuilder.Clear();
							rawBuilder.Clear();
						}
						else
						{
							parametersBuilder.Append(character);
						}
						break;
				}
			}
		}
		
		while (opened.Count > 0)
		{
			Node node = opened.Pop();
			
			textBuilder.Insert(node.Index, $"<{node.Tag}>");
		}
		
		textBuilder.Append(rawBuilder);
		
		m_Text = textBuilder.ToString();
	}
}
