using UnityEngine;
using UnityEngine.UI;

public class SelfGUI : MonoBehaviour {
	
	public Sprite[] marker_sources;
	public Image predicted_point_img;

	private Image marker;

	private Text tgt_dis;
	private Text tgt_vel;

	private Ship player;
	private ShipControl player_script;

	private Ship own_ship;

	private bool is_target = false;
	private bool is_hover = false;

	private bool run = false;

	private void LateStart () {
		own_ship = GetComponent<ShipControl>().myship;
		player_script = GameObject.FindWithTag("Player").GetComponent<ShipControl>();
		player = player_script.myship;

		marker = Instantiate(GameObject.Find("tgt_pos_marker").GetComponent<Image>());
		marker.sprite = marker_sources[0];
		marker.transform.SetParent(GameObject.Find("Canvas").transform);
		marker.rectTransform.anchorMin = Vector2.zero;

		tgt_dis = GameObject.Find("tgt distance").GetComponent<Text>();
		tgt_vel = GameObject.Find("tgt velocity").GetComponent<Text>();
	}

	public void NoTarget () {
		is_target = false;
	}

	private void MainUpdate () {
		Camera cam = SceneGlobals.ship_camera;

		if (SceneGlobals.general.InMap) {
			transform.position = new Vector3(-100, -100);
		}

		if (Vector3.Angle(cam.transform.forward, player.Position - transform.position) > 90f){
			// First project marker on screen
			Vector3 present_pos = cam.WorldToScreenPoint(transform.position);
			present_pos.z = 0f;
			marker.rectTransform.anchoredPosition = present_pos;

			// Check if mouse hovers over the marker
			Vector3 mouse_pos = Input.mousePosition;
			if (present_pos.x-10f < mouse_pos.x && mouse_pos.x < present_pos.x+10f &&
				present_pos.y-10f < mouse_pos.y && mouse_pos.y < present_pos.y+10f){
				// If hovered over marker
				if (!is_hover && !is_target){
					marker.sprite = marker_sources[1];
					is_hover = true;
				}

				if (Input.GetMouseButtonDown(0)){
					// If left mouse Button is pressed
					is_target = !is_target;
					if (is_target){
						marker.sprite = marker_sources[2];
					}
					if (!is_target){
						player.Target = Target.None;
					}
				}
			}
			else{
				if (is_hover && !is_target){
					marker.sprite = marker_sources[0];
					is_hover = false;
				}
			}
		

			if (is_target) {
				//float bullet_speed = 1 / (player.GetComponent<ShipControl>().parts.Weapons[0].BulletSpeed);
				float bullet_speed = 1000f;
				Vector3 player_comp = Vector3.zero;
				Vector3 predicted_point = transform.parent.position + (own_ship.Velocity - player.Velocity)
											/ bullet_speed * Vector3.Distance(transform.position, player.Position) + player_comp;

				Vector3 screen_predicted_point = cam.WorldToScreenPoint(predicted_point);
				screen_predicted_point.z = 0f;

				predicted_point_img.transform.position = screen_predicted_point;

				tgt_dis.text = (player.Velocity - own_ship.Velocity).magnitude.ToString();
				tgt_vel.text = (player.Position - transform.position).magnitude.ToString();
			}
		}
	}

	private void Update () {
		if (GameObject.FindWithTag("Player") != null && !run) {
			LateStart();
			run = true;
		}
		if (run) {
			MainUpdate();
		}
	}

	private void OnDestroy () {
		marker.transform.position = new Vector3(-200, -200);
		predicted_point_img.transform.position = new Vector3(-200, -200);
	}
}