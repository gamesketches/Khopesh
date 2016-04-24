using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

public class BulletDepot : MonoBehaviour {

	public class Bullet{
		public int size;
		public int speed;
		public int damage;
		public int angle;
	}

	public class Volley {
		[XmlArray("Volley")]
		[XmlArrayItem("Bullet")]
		public List<Bullet> volley = new List<Bullet>();
	}

	public class ProjectileType {
		[XmlArray("ProjectileType")]
		[XmlArrayItem("Volley")]
		public List<Volley> volleys = new List<Volley>();

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
	void Start () {
		var serializer = new XmlSerializer(typeof(BulletCollection));
		TextAsset bulletData = Resources.Load("bullets") as TextAsset;
		TextReader reader = new StringReader(bulletData.text);
		types = (BulletCollection)serializer.Deserialize(reader);
	}
}
