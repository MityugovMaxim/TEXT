using NUnit.Framework;
using UnityEngine;

namespace Tests
{
	public class TagTextTest
	{
		[TestCase("<b>TEST", ExpectedResult = "<b>TEST")]
		[TestCase("</b>TEST", ExpectedResult = "</b>TEST")]
		[TestCase("TEST</b>", ExpectedResult = "TEST</b>")]
		[TestCase("TEST<b>", ExpectedResult = "TEST<b>")]
		[TestCase("<b>TEST<b>", ExpectedResult = "<b>TEST<b>")]
		[TestCase("</b>TEST</b>", ExpectedResult = "</b>TEST</b>")]
		[TestCase("</b>TEST<b>", ExpectedResult = "</b>TEST<b>")]
		[TestCase("<b>TEST</b>", ExpectedResult = "TEST")]
		[TestCase("<b><a>TEST</b></a>", ExpectedResult = "<a>TEST</a>")]
		[TestCase("<b><a>TEST</a></b>", ExpectedResult = "TEST")]
		[TestCase("<b a=value>TEST</b>", ExpectedResult = "TEST")]
		[TestCase("<b  a=value>TEST</b>", ExpectedResult = "TEST")]
		[TestCase("<b >TEST</b>", ExpectedResult = "TEST")]
		public string TestText(string _Text)
		{
			TagText tagText = new TagText();
			tagText.Load(_Text);
			return tagText.Text;
		}

		[Test]
		public void TestColorParameters()
		{
			TagText tagText = new TagText();
			tagText.Load("<a b=#FFFFFFFF c=#000000FF d=#FF0000FF e=#00FF00FF f=#0000FFFF>TEST</a>");
			
			Assert.AreEqual("TEST", tagText.Text);
			
			foreach (TagText.Node node in tagText.Nodes)
			{
				TagText.Parameter white = node.Parameters[0];
				Assert.AreEqual("b", white.Name);
				Assert.AreEqual("#FFFFFFFF", white.Value);
				Assert.AreEqual(Color.white, white.GetColor());
				
				TagText.Parameter black = node.Parameters[1];
				Assert.AreEqual("c", black.Name);
				Assert.AreEqual("#000000FF", black.Value);
				Assert.AreEqual(Color.black, black.GetColor());
				
				TagText.Parameter red = node.Parameters[2];
				Assert.AreEqual("d", red.Name);
				Assert.AreEqual("#FF0000FF", red.Value);
				Assert.AreEqual(Color.red, red.GetColor());
				
				TagText.Parameter green = node.Parameters[3];
				Assert.AreEqual("e", green.Name);
				Assert.AreEqual("#00FF00FF", green.Value);
				Assert.AreEqual(Color.green, green.GetColor());
				
				TagText.Parameter blue = node.Parameters[4];
				Assert.AreEqual("f", blue.Name);
				Assert.AreEqual("#0000FFFF", blue.Value);
				Assert.AreEqual(Color.blue, blue.GetColor());
			}
		}

		[Test]
		public void TestIntegerParameters()
		{
			TagText tagText = new TagText();
			tagText.Load("<a b=0 c=-1 d=1>TEST</a>");
			
			Assert.AreEqual("TEST", tagText.Text);
			
			foreach (TagText.Node node in tagText.Nodes)
			{
				TagText.Parameter b = node.Parameters[0];
				Assert.AreEqual("b", b.Name);
				Assert.AreEqual("0", b.Value);
				Assert.AreEqual(0, b.GetInteger());
				
				TagText.Parameter c = node.Parameters[1];
				Assert.AreEqual("c", c.Name);
				Assert.AreEqual("-1", c.Value);
				Assert.AreEqual(-1, c.GetInteger());
				
				TagText.Parameter d = node.Parameters[2];
				Assert.AreEqual("d", d.Name);
				Assert.AreEqual("1", d.Value);
				Assert.AreEqual(1, d.GetInteger());
			}
		}

		[Test]
		public void TestFloatParameters()
		{
			TagText tagText = new TagText();
			tagText.Load("<a b=0.0 c=-1.1 d=1.1>TEST</a>");
			
			Assert.AreEqual("TEST", tagText.Text);
			
			foreach (TagText.Node node in tagText.Nodes)
			{
				TagText.Parameter b = node.Parameters[0];
				Assert.AreEqual("b", b.Name);
				Assert.AreEqual("0.0", b.Value);
				Assert.AreEqual(0.0f, b.GetFloat());
				
				TagText.Parameter c = node.Parameters[1];
				Assert.AreEqual("c", c.Name);
				Assert.AreEqual("-1.1", c.Value);
				Assert.AreEqual(-1.1f, c.GetFloat());
				
				TagText.Parameter d = node.Parameters[2];
				Assert.AreEqual("d", d.Name);
				Assert.AreEqual("1.1", d.Value);
				Assert.AreEqual(1.1f, d.GetFloat());
			}
		}

		[Test]
		public void TestBoolParameters()
		{
			TagText tagText = new TagText();
			tagText.Load("<a b=true c=True d=false e=False>TEST</a>");
			
			Assert.AreEqual("TEST", tagText.Text);
			
			foreach (TagText.Node node in tagText.Nodes)
			{
				TagText.Parameter b = node.Parameters[0];
				Assert.AreEqual("b", b.Name);
				Assert.AreEqual("true", b.Value);
				Assert.AreEqual(true, b.GetBool());
				
				TagText.Parameter c = node.Parameters[1];
				Assert.AreEqual("c", c.Name);
				Assert.AreEqual("True", c.Value);
				Assert.AreEqual(true, c.GetBool());
				
				TagText.Parameter d = node.Parameters[2];
				Assert.AreEqual("d", d.Name);
				Assert.AreEqual("false", d.Value);
				Assert.AreEqual(false, d.GetBool());
				
				TagText.Parameter e = node.Parameters[3];
				Assert.AreEqual("e", e.Name);
				Assert.AreEqual("False", e.Value);
				Assert.AreEqual(false, e.GetBool());
			}
		}
	}
}