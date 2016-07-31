#pragma strict
public var raptor : boolean = true;

function Start () {
GameObject.Find("leftButton").GetComponent.<GUITexture>().pixelInset = Rect(
		Screen.width*0.2,
		Screen.height*0.1,
		Screen.width*0.05, 
		Screen.width*0.05);
		
GameObject.Find("rightButton").GetComponent.<GUITexture>().pixelInset = Rect(
		Screen.width*0.8,
		Screen.height*0.1,
		Screen.width*0.05, 
		Screen.width*0.05);	

GameObject.Find("helpbutton").GetComponent.<GUITexture>().pixelInset = Rect(
		Screen.width*0.9,
		Screen.height*0.8,
		Screen.width*0.06, 
		Screen.width*0.06);	

GameObject.Find("animationList").GetComponent.<GUITexture>().pixelInset = Rect(
		Screen.width/2-Screen.height*0.9*422/292/2,
		Screen.height*0.05,
		Screen.height*0.9*422/292, 
		Screen.height*0.9);			
		
}

function OnMouseDown () {

if (GameObject.Find("animationList").GetComponent.<GUITexture>().enabled == false) GameObject.Find("animationList").GetComponent.<GUITexture>().enabled = true;
else GameObject.Find("animationList").GetComponent.<GUITexture>().enabled = false;

}