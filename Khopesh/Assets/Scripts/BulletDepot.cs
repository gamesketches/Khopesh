using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

public class BulletDepot : ScriptableObject {

	public class Bullet{
		public float size;
		public float speed;
		public int damage;
		public int angle;
	}

	public class Volley {
		[XmlElement("Bullet")]
		public Bullet[] volley;
	}

	public class ProjectileType {
		[XmlElement("Volley")]
		public Volley[] volleys;

		[XmlAttribute("name")]
		public string typeName;
	}

	[XmlRoot("Root")]
	public class BulletCollection {
		[XmlArray("BulletCollection")]
		[XmlArrayItem("ProjectileType")]
		public List<ProjectileType> projectileTypes = new List<ProjectileType>();
	}

	public BulletCollection types;
	// Use this for initialization
	public void Load () {
		var serializer = new XmlSerializer(typeof(BulletCollection));
		TextAsset bulletData = Resources.Load("bullets") as TextAsset;
		TextReader reader = new StringReader(bulletData.text);
		types = (BulletCollection)serializer.Deserialize(reader);
	}
}
