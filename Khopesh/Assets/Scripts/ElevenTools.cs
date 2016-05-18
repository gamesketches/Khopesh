using UnityEngine;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// Copyright (c) 2015 11:11 Studios LLC
/// 
/// Permission is hereby granted, free of charge, to any person obtaining a copy of this 
/// software and associated documentation files (the "Software"), to deal in the Software 
/// without restriction, including without limitation the rights to use, copy, modify, merge, 
/// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons 
/// to whom the Software is furnished to do so, subject to the following conditions:
///
/// The above copyright notice and this permission notice shall be included 
/// in all copies or substantial portions of the Software.
/// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING 
/// BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND 
/// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, 
/// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
/// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
/// </summary>
public static class ElevenTools 
{
	public const float TwoPI = 2 * Mathf.PI;

	public static string GetDocumentsPath () 
	{ 
		return Application.persistentDataPath;
		/*
#if UNITY_EDITOR
		return Directory.GetParent(Application.dataPath).FullName + "/Documents";
#else
		// Your game has read+write access to /var/mobile/Applications/XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX/Documents 
		// Application.dataPath returns              
		// /var/mobile/Applications/XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX/myappname.app/Data 
		// Strip "/Data" from path 
		string path = Directory.GetParent(Directory.GetParent(Application.dataPath).FullName).FullName; 
		return path + "/Documents"; 
#endif
		*/
	}

	public static Color NewAlpha(this Color color, float alpha) {
		return new Color(color.r, color.g, color.b, alpha);
	}

	public static bool HasParent(this Transform transform, Transform potentialParent) {
		if (transform.parent == null) {
			if (potentialParent == null) {
				return true;
			} else {
				return false;
			}
		} else if (transform.parent == potentialParent) {
			return true;
		} else {
			return transform.parent.HasParent(potentialParent);
		}
	}
	
	public static void ParentTo(this Transform transform, Transform newParent, 
	                            Vector3 localPos, 
	                            Quaternion localRot, 
	                            Vector3 localScale)
	{
		transform.SetParent(newParent);
		transform.localPosition = localPos;
		transform.localRotation = localRot;
		transform.localScale = localScale;	
	}
	
	public static Rect Multiply(this Rect rectangle, Vector2 vec)
	{
		return new Rect(rectangle.x * vec.x, rectangle.y * vec.y, 
			rectangle.width * vec.x, rectangle.height * vec.y);
	}
	
	public static Rect ToPixels(this Rect screenPercent) {
		return new Rect(Screen.width * screenPercent.x, Screen.height * screenPercent.y,
			Screen.width * screenPercent.width, Screen.height * screenPercent.height);
	}
	
	public static void SetChildren(this Transform transform, bool isActive)
	{
		for (int i = 0; i < transform.childCount; i++)
		{
			transform.GetChild(i).gameObject.SetActive(isActive);	
		}
	}
	
	public static List<T> ToList<T>(this ArrayList arrayList)
    {
		List<T> list = new List<T>(arrayList.Count);
		foreach (T instance in arrayList)
		{
		    list.Add(instance);
		}
		return list;
    }
	
	public static string RandomString(int size)
	{
        StringBuilder builder = new StringBuilder();
        char ch;
        for (int i = 0; i < size; i++)
        {
            ch = (char)((int)(Mathf.Floor(26 * Random.Range(0.0f, 1.0f) + 65)));                 
            builder.Append(ch);
        }

        return builder.ToString();
    }
	
	//const string ALLOWED_CHARACTERS = @"[\W]";//@"^[a-zA-Z0-9\s,]*$";
	const string DISALLOWED_CHARACTERS = "[^0-9A-Za-z ]+$";//@"\W";//@"[a-zA-Z0-9\s,]*$";

	public static bool IsAlphaNumeric(string strToCheck)
	{
		return !Regex.IsMatch(strToCheck, DISALLOWED_CHARACTERS);
	}

	public static bool ContainsWhiteSpace(string strToCheck) 
	{
		return Regex.IsMatch(strToCheck, @"\s+");
	}
	
	public static string ReplaceNonAlphaNumeric(this string str)
	{
		return Regex.Replace(str, DISALLOWED_CHARACTERS, string.Empty);
	}
	
	public static float SinInterpolation(float start, float end, float time, float power = 0.7f) 
	{
		if (time < 0.0f)
			return start;
		else if (time >= 1.0f)
			return end;
		
		// does sin function from 0 to 1
		float amt = Mathf.Pow(.5f + .5f * Mathf.Sin( Mathf.PI * (time - .5f) ), power);
		
		return (1 - amt) * start + amt * end;		
	}
	
	public static Vector3 SinInterpolation(Vector3 start, Vector3 end, float time, float power = 0.7f)
	{
		Vector3 toReturn = new Vector3();
		for (int i = 0; i < 3; i++) {
			toReturn[i] = SinInterpolation(start[i], end[i], time, power);
		}
		return toReturn;
	}

	public static Color SinInterpolation(Color start, Color end, float time, float power = 0.7f)
	{
		Color toReturn = new Color();
		for (int i = 0; i < 4; i++) {
			toReturn[i] = SinInterpolation(start[i], end[i], time, power);
		}
		return toReturn;
	}

	public static float WrapAngle(float angle)
	{
		return angle - TwoPI * Mathf.Floor(angle / TwoPI);
	}

	public static void Shuffle(this int[] toShuffle) 
	{
		for (int i = toShuffle.Length - 1; i >= 1; i--) {
			int j = Random.Range(0, i + 1);
			int tmp = toShuffle[i];
			toShuffle[i] = toShuffle[j];
			toShuffle[j] = tmp;
		}
	}

	public static Vector3 Project(Vector3 vector, Vector3 projectOn) {
		return projectOn * Mathf.Max(0.0f, Vector3.Dot(vector, projectOn));	
	}
	
	public static Vector3 Project(Vector3 position, Vector3 segmentA, Vector3 segmentB) {
		// find the position we are orthogonal at with a projection vector
		// Reference: http://en.wikipedia.org/wiki/Vector_projection
		Vector3 a = position - segmentA;
		Vector3 b = segmentB - segmentA;
		
		// we calculate the length and make it not possible to be negative
		float length = Mathf.Clamp(Vector3.Dot(a, b) / Vector3.Dot(b, b), 0.0f, 1.0f);		
		Vector3 c = b * length;
		
		return segmentA + c;		
	}
	
	public static bool IsInfinity(this Vector3 vec) {
		return float.IsInfinity(vec.x) || float.IsInfinity(vec.y) || float.IsInfinity(vec.z);	
	}
	
	
	public static Vector3 ToVector3( this string sourceString )
	{
		string[] axisStrings = sourceString.Split(',');
		if(axisStrings.Length != 3)
		{
			return Vector3.zero;
		}
		
		return new Vector3(float.Parse(axisStrings[0]), float.Parse(axisStrings[1]), float.Parse(axisStrings[2]));
	}
	
	public static Quaternion ToQuaternion( this string sourceString )
	{
		string[] axisStrings = sourceString.Split(',');
		if(axisStrings.Length != 4)
		{
			return Quaternion.identity;
		}
		
		return new Quaternion(float.Parse(axisStrings[0]), float.Parse(axisStrings[1]), float.Parse(axisStrings[2]), float.Parse(axisStrings[3]));
	}
	

	public static bool Approximately( this Vector2 v, Vector2 other )
	{
		return
			Mathf.Approximately( v.x, other.x ) &&
			Mathf.Approximately( v.y, other.y );
	}

	public static bool Approximately( this Vector3 v, Vector3 other )
	{
		return
			Mathf.Approximately( v.x, other.x ) &&
				Mathf.Approximately( v.y, other.y ) &&
				Mathf.Approximately( v.z, other.z );
	}
	
	public static bool Approximately( this Rect r, Rect other )
	{
		return
			Mathf.Approximately( r.x, other.x ) &&
				Mathf.Approximately( r.y, other.y ) &&
				Mathf.Approximately( r.width, other.width ) &&
				Mathf.Approximately( r.height, other.height );
	}
	
	public static bool Approximately( this Vector3 v, Vector3 other, float maxDeviation )
	{
		return Vector3.SqrMagnitude( v - other ) < (maxDeviation*maxDeviation);
	}
	
	public static Vector3 Sign( this Vector3 v )
	{
		return new Vector3( Mathf.Sign( v.x ), Mathf.Sign( v.y ), Mathf.Sign( v.z ) );
	}
	
	public static bool Approximately( this Quaternion q, Quaternion other )
	{
		return
			Mathf.Approximately( q.x, other.x ) &&
				Mathf.Approximately( q.y, other.y ) &&
				Mathf.Approximately( q.z, other.z ) &&
				Mathf.Approximately( q.w, other.w );
	}
	
	public static bool Approximately( this Quaternion q, Quaternion other, float maxDeviation )
	{
		return
			Mathf.Abs( q.x - other.x ) < maxDeviation &&
				Mathf.Abs( q.y - other.y ) < maxDeviation &&
				Mathf.Abs( q.z - other.z ) < maxDeviation &&
				Mathf.Abs( q.w - other.w ) < maxDeviation;
	}
	
	public static bool IsEnclosing( this Rect rect, Rect other )
	{
		return rect.Contains( new Vector2( other.x, other.y ) ) && rect.Contains( new Vector2( other.x + other.width, other.y + other.height ) );
	}
	
	public static bool IsOverlapping( this Rect rect, Rect other )
	{
		if( rect.x > other.x + other.width || other.x > rect.x + rect.width )
			return false;
		if( rect.y > other.y + other.height || other.y > rect.y + rect.height )
			return false;
		
		return true;
		//return ((Mathf.Abs( other.x - rect.x ) <= rect.width + other.width )) &&
		//    (Mathf.Abs( other.y - rect.y ) <= rect.height + other.height );
	}

	public static Rect Lerp(Rect rectA, Rect rectB, float amt) {
		Rect toReturn = new Rect();
		toReturn.x = Mathf.Lerp(rectA.x, rectB.x, amt);
		toReturn.y = Mathf.Lerp(rectA.y, rectB.y, amt);
		toReturn.width = Mathf.Lerp(rectA.width, rectB.width, amt);
		toReturn.height = Mathf.Lerp(rectA.height, rectB.height, amt);
		return toReturn;
	}

	public static Vector3 RandomPoint(Bounds bounds) {
		return RandomPoint(bounds.min, bounds.max);
	}

	public static Vector3 RandomPoint(Vector3 min, Vector3 max) {
		return new Vector3(Random.Range(min.x, max.x), Random.Range(min.y, max.y), Random.Range(min.z, max.z));
	}

	public static Vector2 RandomPoint(this Rect rect) {
		return new Vector2(Random.Range(rect.xMin, rect.xMax), Random.Range(rect.yMin, rect.yMax));
	}

	public static Color RandomColor() {
		return new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
	}

	public static Color Oscillate(Color colorA, Color colorB, float amt) {
		
		return Color.Lerp(colorA, colorB, 
		                  0.5f + 0.5f * Mathf.Cos(ElevenTools.TwoPI * amt));
	}

	public static float Oscillate(float a, float b, float amt) {
		return Mathf.Lerp(a, b, 
		                  0.5f + 0.5f * Mathf.Cos(ElevenTools.TwoPI * amt));
	}

	public static string ToHexString(this Color color) {
		StringBuilder sb = new StringBuilder();
		for (int i = 0; i < 3; i++) {
			string val = System.Convert.ToString((int)(255 * color[i]), 16);
			for (int j = val.Length; j < 2; j++) {
				val += "0";
			}
			sb.Append(val);
		}
		
		return sb.ToString();
	}

	public static bool IsInLayerMask(GameObject obj, LayerMask mask){
		return mask == (mask | (1 << obj.layer));
	}
	
	public static void FillParent(this RectTransform rectTransform) {
		rectTransform.FillParent(new Vector2(0, 0), new Vector2(1, 1));	
	}

	public static void FillParent(this RectTransform rectTransform, Vector2 min, Vector2 max) {
		rectTransform.localPosition = Vector3.zero;
		rectTransform.localScale = Vector3.one;
		rectTransform.offsetMin = Vector2.zero;
		rectTransform.offsetMax = Vector2.one;
	}

	public static Vector2 OnUnitCircle() {
		float angle = Random.Range(-Mathf.PI, Mathf.PI);
		return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
	}

	public static Vector3 ToVector3(this Vector2 vector, float zValue = 0.0f) {
		return new Vector3(vector.x, vector.y, zValue);
	}

	public static Vector3 NewX(this Vector3 vector, float xValue = 0.0f) {
		return new Vector3(xValue, vector.y, vector.z);
	}

	public static Vector3 NewY(this Vector3 vector, float yValue = 0.0f) {
		return new Vector3(vector.x, yValue, vector.z);
	}

	public static Vector3 NewZ(this Vector3 vector, float zValue = 0.0f) {
		return new Vector3(vector.x, vector.y, zValue);
	}

	public static Vector4 ToVector4(this Vector3 vector, float wValue = 1.0f) {
		return new Vector4(vector.x, vector.y, vector.z, wValue);
	}
	
	public static void Shuffle<T>(this IList<T> list, int seed)
	{
		System.Random rng = new System.Random(seed);  
		int n = list.Count;  
		while (n > 1) {  
			n--;  
			int k = rng.Next(n + 1);  
			T value = list[k];  
			list[k] = list[n];  
			list[n] = value;  
		}  
	}
	
	public static void Shuffle<T>(this IList<T> list)
	{
		System.Random rng = new System.Random();  
		int n = list.Count;  
		while (n > 1) {  
			n--;  
			int k = rng.Next(n + 1);  
			T value = list[k];  
			list[k] = list[n];  
			list[n] = value;  
		}  
	}

	public static Component CopyComponent(Component original, GameObject destination)
	{
		System.Type type = original.GetType();
		Component copy = destination.AddComponent(type);
		// Copied fields can be restricted with BindingFlags
		System.Reflection.FieldInfo[] fields = type.GetFields(); 
		foreach (System.Reflection.FieldInfo field in fields)
		{
			Debug.Log(field.Name);
			field.SetValue(copy, field.GetValue(original));
		}
		return copy;
	}

	public static string ToScoreBoardTime(this float timeLeft, string singleDigit = "0.0") {
		string timeLeftStr;
		if (timeLeft >= 10.0f) {
			timeLeftStr = ((int)timeLeft / 60) + ":" + ((int)timeLeft % 60).ToString("00");
		} else {
			timeLeftStr = timeLeft.ToString(singleDigit);			
		}
		return timeLeftStr;
	}

	public static float Smooth(float t) {
		t = Mathf.Clamp(t, 0.0f, 1.0f);
		return t * t * ( 3.0f - (2.0f * t) );
	}

	public static float SmoothStep(float a, float b, float x) {
		float t = (x - a) / (b - a);
		return Smooth(t);
	}

	public static float SmoothLerp(float a, float b, float t) {
		return Mathf.Lerp(a, b, t);
	}

	public static Vector3 Abs(Vector3 val) {
		return new Vector3(Mathf.Abs(val.x), Mathf.Abs(val.y), Mathf.Abs(val.z));
	}
}
