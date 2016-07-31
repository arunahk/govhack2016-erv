#pragma strict
var target : GameObject ;
var aniMax : int ;
var aniMin : int ;

function OnMouseDown () {
if(this.name == "rightButton" ) target.GetComponent(AnimationManager).NextAnim();
else target.GetComponent(AnimationManager).PrevAnim();

if (target.GetComponent(Animator).GetInteger("animer") == 0 && this.name == "leftButton")
	this.GetComponent.<GUITexture>().enabled = false;
if (target.GetComponent(Animator).GetInteger("animer") > 0){
	print (target.GetComponent(Animator).GetInteger("animer"));
	GameObject.Find("leftButton").GetComponent.<GUITexture>().enabled = true;
	}

if (target.GetComponent(Animator).GetInteger("animer") == 14 && this.name == "rightButton")
	this.GetComponent.<GUITexture>().enabled = false;
if (target.GetComponent(Animator).GetInteger("animer") < 14 )
	GameObject.Find("rightButton").GetComponent.<GUITexture>().enabled = true;


}
