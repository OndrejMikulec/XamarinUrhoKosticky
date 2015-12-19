using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Timers;

using Urho;
using Urho.Resources;
using Urho.Gui;
using Urho.Physics;



namespace UrhoKosticky
{
	public class Kosticky : Application
	{
		Scene scene;
		Node CameraNode;

		float Yaw { get; set; }
		float Pitch { get; set; }
		float Roll { get; set; }
		bool TouchEnabled { get; set; }
		const float TouchSensitivity = 2;
		const float PixelSize = 0.01f;

		Text textInstrukce;
		Text textboxesCount;
		Timer nabijeni;
		bool nabito = true;

		List<Node> boxesNodesList = new List<Node> ();
		List<Node> ballNodesList = new List<Node> ();

		Vector3 cameraStartPosition = new Vector3 (0.0f, 5.0f, -20.0f);


		//0=idle
		int gameStateNow = 1;

		List<int> gameStatesForNewBoxesBuilded;
		List<int> gameStatesForNewBoxes(int startState, int rate, int max) 
		{
			List<int> returnList = new List<int> ();
			int rateCount = rate;
			for (int i = startState; i < max*rate; i++) {
				if (rateCount==rate) {
					returnList.Add (i);
					rateCount = 0;
				}
				rateCount++;
			}
			return returnList;

		}

		protected override void Start()
		{
			gameStatesForNewBoxesBuilded = gameStatesForNewBoxes(7, 10, 20) ;

			
			Graphics.SetWindowIcon(ResourceCache.GetImage ("Textures/UrhoIcon.png"));
			Graphics.WindowTitle = "PF 2016";

			nabijeni = new Timer (500);
			nabijeni.Elapsed += delegate {
				nabito = true;
			};

			base.Start();
			CreateScene();
			SetupViewport();

			initInstructionsText ("Ahoj"+Environment.NewLine+"Rozbi zed.");
			initBoxesCountText ("Zbývá "+boxesNodesList.Count+" kostiček.");

			initControls ();

		}


		protected override void OnUpdate(float timeStep)
		{
			base.OnUpdate(timeStep);
			SimpleMoveCamera3D(timeStep);
			MoveCameraByTouches(timeStep);

			if (gameStateNow == 1) {
				destroyStateCheck ();

			} else if (gameStateNow == 2){
				waitAndSetGameAction (3, 2000);

			} else if (gameStateNow == 3) {
				endCleanGarbageAction ();
				gameStateNow = 4;

			} else if (gameStateNow == 4){
				moveCameraToFinal (cameraStartPosition,0,0,5);

			}  else if (gameStateNow == 5){
				waitAndSetGameAction (6, 2000);

			} else if (gameStateNow == 6){
				endCleanGarbageAction ();
				boxesNodesList.Clear ();
				gameStateNow = gameStatesForNewBoxesBuilded[0];
			} else if (gameStateNow >= gameStatesForNewBoxesBuilded[0] && gameStateNow <= gameStatesForNewBoxesBuilded[gameStatesForNewBoxesBuilded.Count-1]){
				if (gameStatesForNewBoxesBuilded.Contains (gameStateNow)) {
					createBoxes ();
				}
				gameStateNow++;
			}


		}

		void initControls()
		{
			string WithMyButton =
				"<patch>" +
				"    <remove sel=\"/element/element[./attribute[@name='Name' and @value='Button1']]/attribute[@name='Is Visible']\" />" +
				"    <replace sel=\"/element/element[./attribute[@name='Name' and @value='Button1']]/element[./attribute[@name='Name' and @value='Label']]/attribute[@name='Text']/@value\">Fire</replace>" +
				"    <add sel=\"/element/element[./attribute[@name='Name' and @value='Button1']]\">" +
				"        <element type=\"Text\">" +
				"            <attribute name=\"Name\" value=\"KeyBinding\" />" +
				"            <attribute name=\"Text\" value=\"SPACE\" />" +
				"        </element>" +
				"    </add>" +
				"</patch>";

			TouchEnabled = true;
			var layout = ResourceCache.GetXmlFile("UI/ScreenJoystick_Samples.xml");
			XmlFile patchXmlFile = new XmlFile();
			patchXmlFile.FromString(WithMyButton);
			layout.Patch(patchXmlFile);
			var screenJoystickIndex = Input.AddScreenJoystick(layout, ResourceCache.GetXmlFile("UI/DefaultStyle.xml"));
			Input.SetScreenJoystickVisible(screenJoystickIndex, true);			
		}

		void SimpleMoveCamera3D (float timeStep, float moveSpeed = 10.0f)
		{
			const float mouseSensitivity = .1f;

			if (UI.FocusElement != null)
				return;

			var mouseMove = Input.MouseMove;
			Yaw += mouseSensitivity * mouseMove.X;
			Pitch += mouseSensitivity * mouseMove.Y;
			Pitch = MathHelper.Clamp(Pitch, -90, 90);

			CameraNode.Rotation = new Quaternion(Pitch, Yaw, 0);


			if (Input.GetKeyDown (Key.W)) CameraNode.Translate ( Vector3.UnitZ * moveSpeed * timeStep);
			if (Input.GetKeyDown (Key.S)) CameraNode.Translate (-Vector3.UnitZ * moveSpeed * timeStep);
			if (Input.GetKeyDown (Key.A)) CameraNode.Translate (-Vector3.UnitX * moveSpeed * timeStep);
			if (Input.GetKeyDown (Key.D)) CameraNode.Translate ( Vector3.UnitX * moveSpeed * timeStep);
			if (Input.GetKeyDown (Key.Space)) {
				textInstrukce.Value = "";
				if (nabito) {
					nabito = false;
					SpawnObject ();
					nabijeni.Start ();

					if (textInstrukce.Value != null) {
						textInstrukce.Value = null;
					}

				}
			}

		}


		void MoveCameraByTouches (float timeStep)
		{
			if (!TouchEnabled || CameraNode == null)
				return;

			if (UI.FocusElement != null)
				return;


			var input = Input;
			for (uint i = 0, num = input.NumTouches; i < num; ++i)
			{
				TouchState state = input.GetTouch(i);
				if (state.TouchedElement != null)
					continue;

				if (state.Delta.X != 0 || state.Delta.Y != 0)
				{
					var camera = CameraNode.GetComponent<Camera>();
					if (camera == null)
						return;

					var graphics = Graphics;
					Yaw += TouchSensitivity * camera.Fov / graphics.Height * state.Delta.X;
					Pitch += TouchSensitivity * camera.Fov / graphics.Height * state.Delta.Y;
					CameraNode.Rotation = new Quaternion(Pitch, Yaw, 0);

				}

			}
		}


		void initInstructionsText(string text = "")
		{
			textInstrukce = new Text()
			{
				Value = text,
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center
			};
			textInstrukce.SetFont(ResourceCache.GetFont("Fonts/Anonymous Pro.ttf"), 20);
			textInstrukce.SetColor(new Color(0f, 1f, 0f));
			UI.Root.AddChild(textInstrukce);
		}

		void initBoxesCountText(string text = "")
		{
			textboxesCount = new Text() 
			{
				Value = text,
				HorizontalAlignment = HorizontalAlignment.Left,
				VerticalAlignment = VerticalAlignment.Center
			};
			textboxesCount.SetFont(ResourceCache.GetFont("Fonts/Anonymous Pro.ttf"), 20);
			textboxesCount.SetColor(new Color(0f, 1f, 0f));
			UI.Root.AddChild(textboxesCount);
		}

		void SetupViewport()
		{
			var renderer = Renderer;
			renderer.SetViewport(0, new Viewport(Context, scene, CameraNode.GetComponent<Camera>(), null));
		}

		void CreateScene()
		{
			var cache = ResourceCache;
			scene = new Scene();

			scene.CreateComponent<Octree>();
			scene.CreateComponent<PhysicsWorld>();
			scene.CreateComponent<DebugRenderer>();

			Node zoneNode = scene.CreateChild("Zone");
			Zone zone = zoneNode.CreateComponent<Zone>();
			zone.SetBoundingBox(new BoundingBox(-1000.0f, 1000.0f));
			zone.AmbientColor=new Color(0.15f, 0.15f, 0.15f);
			zone.FogColor=new Color(1.0f, 1.0f, 1.0f);
			zone.FogStart=300.0f;
			zone.FogEnd=500.0f;

			Node lightNode = scene.CreateChild("DirectionalLight");
			lightNode.SetDirection(new Vector3(0.6f, -1.0f, 0.8f));
			Light light = lightNode.CreateComponent<Light>();
			light.LightType=LightType.Directional;
			light.CastShadows=true;
			light.ShadowBias=new BiasParameters(0.00025f, 0.5f);

			light.ShadowCascade=new CascadeParameters(10.0f, 50.0f, 200.0f, 0.0f, 0.8f);


			Node skyNode = scene.CreateChild("Sky");
			skyNode.SetScale(500.0f); // The scale actually does not matter
			Skybox skybox = skyNode.CreateComponent<Skybox>();
			skybox.Model=cache.GetModel("Models/Box.mdl");
			skybox.SetMaterial(cache.GetMaterial("Materials/Skybox.xml"));

			{
				Node floorNode = scene.CreateChild("Floor");
				floorNode.Position=new Vector3(0.0f, -0.5f, 0.0f);
				floorNode.Scale=new Vector3(1000.0f, 1.0f, 1000.0f);
				StaticModel floorObject = floorNode.CreateComponent<StaticModel>();
				floorObject.Model=cache.GetModel("Models/Box.mdl");
				floorObject.SetMaterial(cache.GetMaterial("Materials/Terrain.xml"));


				floorNode.CreateComponent<RigidBody>(); 
				CollisionShape shape = floorNode.CreateComponent<CollisionShape>();
				shape.SetBox(Vector3.One, Vector3.Zero, Quaternion.Identity);
			}


			for (int x = 0; x <= 14; x++) {
				for (int z = 0; z <= 0; z++) {
					for (int y = 0; y <= 7; y++) {

						createBoxAndAddToList (
							new Vector3 ((float)x + 0.3f - 6f, (float)y + 0.3f, (float)z + 0.3f), 
							boxesNodesList
						);
					}
				}
			}

			colorBoxesByColorSchema (boxesNodesList, BoxColorSchemas.boxes2 (-5, 2), cache.GetMaterial ("Materials/M_0020_Red.xml"));
			colorBoxesByColorSchema (boxesNodesList, BoxColorSchemas.boxes0 (-1, 2), cache.GetMaterial ("Materials/M_0060_GrassGreen.xml"));
			colorBoxesByColorSchema (boxesNodesList, BoxColorSchemas.boxes1 (3, 2), cache.GetMaterial ("Materials/M_0056_Yellow.xml"));
			colorBoxesByColorSchema (boxesNodesList, BoxColorSchemas.boxes5 (5, 2), cache.GetMaterial ("Materials/M_0103_Blue.xml"));

			CameraNode = new Node();
			Camera oCamera = CameraNode.CreateComponent<Camera>();
			oCamera.FarClip = 500.0f;

			CameraNode.Position = cameraStartPosition;

		}

		public void createBoxAndAddToList(Vector3 position, List<Node> listToAdd)
		{
			var cache = ResourceCache;
			Node boxNode = scene.CreateChild("Box");
			boxNode.Position = position;
			StaticModel boxObject = boxNode.CreateComponent<StaticModel>();
			boxObject.Model=cache.GetModel("Models/Box.mdl");
			boxObject.SetMaterial (cache.GetMaterial ("Materials/M_0132_LightGray.xml"));
			boxObject.CastShadows = true;
			RigidBody body = boxNode.CreateComponent<RigidBody>();
			body.Mass=1f;
			body.Friction=1f;
			CollisionShape shape = boxNode.CreateComponent<CollisionShape>();
			shape.SetBox(Vector3.One, Vector3.Zero, Quaternion.Identity);

			listToAdd.Add (boxNode);
		}

		void colorBoxesByColorSchema(List<Node> boxesListToColor, List<int[]> schema, Material mat)
		{
			foreach (Node item in boxesListToColor) {
				int[] roundedPosition = new int[] {
					(int)Math.Round (item.Position.X),
					(int)Math.Round (item.Position.Y),
					(int)Math.Round (item.Position.Z)
				};
				if (vectorsListContains(schema,roundedPosition)) {
					StaticModel boxObject = item.GetComponent<StaticModel> ();
					boxObject.SetMaterial (mat);					
				}
			}
		}


		void SpawnObject()
		{
			var cache = ResourceCache;

			var boxNode = scene.CreateChild("SmallBox");
			boxNode.Position = CameraNode.Position;
			boxNode.Rotation = CameraNode.Rotation;
			boxNode.SetScale(0.5f);

			StaticModel boxModel = boxNode.CreateComponent<StaticModel>();
			boxModel.Model = cache.GetModel("Models/Sphere.mdl");
			boxModel.SetMaterial(cache.GetMaterial("Materials/StoneEnvMapSmall.xml"));
			boxModel.CastShadows = true;

			var body = boxNode.CreateComponent<RigidBody>();
			body.Mass = 0.2f;
			body.Friction = 0.5f;
			var shape = boxNode.CreateComponent<CollisionShape>();
			shape.SetBox(Vector3.One, Vector3.Zero, Quaternion.Identity);

			const float objectVelocity = 40.0f;

			body.SetLinearVelocity(CameraNode.Rotation * new Vector3(0f, 0.25f, 1f) * objectVelocity);

			ballNodesList.Add (boxNode);
		}

		bool vectorsComparer(int[] vec1, int[] vec2)
		{
			if (vec1[0]==vec2[0]&&vec1[1]==vec2[1]&&vec1[2]==vec2[2]) {
				return true;
			}
			return false;
		}

		bool vectorsListContains(List<int[]> li,int[] member )
		{
			foreach (int[] item in li) {
				if (vectorsComparer (item,member)) {
					return true;
				}
			}
			return false;
		}

		bool destroyStateCheck()
		{
			var cache = ResourceCache;

			bool returnBool = true;
			int count = 0;
			foreach (Node item in boxesNodesList) {
				if (item.Position.Y>2) {
					count++;
					returnBool = false;

				} else {
					item.GetComponent<StaticModel> ().SetMaterial(cache.GetMaterial("Materials/M_0039_DarkOrange.xml"));
				}
			}
			textboxesCount.Value =  ("Zbývá zbořit"+Environment.NewLine+count+" kostiček.");

			if (count==0) {
				gameStateNow = 2;
			}

			return returnBool;
		}

		Timer waitAfterGarbage = new Timer();
		void waitAndSetGameAction(int nextState, int interval)
		{
			gameStateNow = 0;
			waitAfterGarbage.Interval = interval;
			waitAfterGarbage.AutoReset = false;
			waitAfterGarbage.Elapsed += delegate {
				gameStateNow = nextState;
			};
			waitAfterGarbage.Start ();

		}

		void endCleanGarbageAction()
		{
			Random dir = new Random ();

			foreach (Node item in boxesNodesList) {
				endMove (item,dir);
			}

			foreach (Node item in ballNodesList) {
				endMove (item,dir);
			}				
		}

		void endMove(Node item,Random dir )
		{
			StringHash oStringHash = new StringHash (RigidBody.TypeStatic.Code);

			RigidBody oRigidBody = (RigidBody)item.GetComponent (oStringHash, false);


			float dirX = ((float)dir.Next (-1000, 1000))/ 1000;
			float dirY = ((float)dir.Next (0, 1000)) / 1000;
			float dirZ = ((float)dir.Next (-1000, 1000)) / 1000;

			item.GetComponent<RigidBody> ().SetLinearVelocity (new Vector3(dirX,dirY,dirZ)*50);

		}

		bool getPitchedteSteps = false;
		bool getYawedteSteps = false;
		bool getMoveSteps = false;

		bool movedInPosition = false;
		bool yawedInPosition = false;
		bool pitchedInPosition = false;

		float stepX = 0;
		float stepY = 0;
		float stepZ = 0;

		float stepYaw = 0;
		float stepPitch = 0;

		void moveCameraToFinal(Vector3 targetPosition,float targetPitch, float targetYaw, int nextGameSate)
		{

			if (!pitchedInPosition) {
				if (!getPitchedteSteps) {
					getPitchedteSteps = true;

					stepPitch = ( targetPitch - CameraNode.Rotation.PitchAngle )/10; 
				}

				if (Math.Round(CameraNode.Rotation.PitchAngle,1)!=Math.Round(targetPitch,1)	) {
					Pitch += stepPitch;

					CameraNode.Rotation = new Quaternion (Pitch,0,0);

				} else {
					pitchedInPosition = true;
				}				
			}

			if (pitchedInPosition&&!yawedInPosition) {
				if (!getYawedteSteps) {
					getYawedteSteps = true;

					stepYaw = ( CameraNode.Rotation.YawAngle -targetYaw)/10; 

				}

				if (	Math.Round(CameraNode.Rotation.YawAngle,1)!=Math.Round(targetYaw,1)	) {
					Yaw += stepYaw;

					CameraNode.Rotation = new Quaternion (0,Yaw,0);

				} else {
					yawedInPosition = true;
				}				
			}

			if (yawedInPosition&&!movedInPosition) {
				if (!getMoveSteps) {
					getMoveSteps = true;
					stepX = ( targetPosition.X - CameraNode.Position.X)/100; 
					stepY = ( targetPosition.Y - CameraNode.Position.Y)/100; 
					stepZ = ( targetPosition.Z - CameraNode.Position.Z)/100; 

				}

				if (	Math.Round(targetPosition.X,1)!=Math.Round( CameraNode.Position.X,1)
					|| 	Math.Round(targetPosition.Y,1)!=Math.Round( CameraNode.Position.Y,1)
					|| 	Math.Round(targetPosition.Z,1)!=Math.Round( CameraNode.Position.Z,1)
				) {

				float cameraPositionX = CameraNode.Position.X + stepX;
					float cameraPositionY = CameraNode.Position.Y + stepY;
					float cameraPositionZ = CameraNode.Position.Z + stepZ;

				CameraNode.Position = new Vector3(cameraPositionX,cameraPositionY,cameraPositionZ);
				} else {
					movedInPosition = true;
				}			
			}

			if (	movedInPosition == true
				&& 	yawedInPosition == true
				&& 	pitchedInPosition == true) {
				gameStateNow = nextGameSate;
			}


		}

		void createBoxes ()
		{
			for (int i = 0; i < 1; i++) {
				createBoxAndAddToList (new Vector3 (0f, 50f, 0f), boxesNodesList);
			}
		}

	}
}

