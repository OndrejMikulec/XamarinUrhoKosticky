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

		static List<Material> matRandomList = new List<Material> () ;
		Material matRandomizer()
		{
			if (matRandomList.Count==0) {
				matRandomList.Add (ResourceCache.GetMaterial (Assets.Materials.yellow) );
				matRandomList.Add (ResourceCache.GetMaterial (Assets.Materials.red));
				matRandomList.Add (ResourceCache.GetMaterial (Assets.Materials.green));
				matRandomList.Add (ResourceCache.GetMaterial (Assets.Materials.blue));
			}

			Material returnMat;
			Random oRandom = new Random ();
			returnMat = matRandomList[oRandom.Next (0, matRandomList.Count - 1)];
			matRandomList.Remove (returnMat);
			return returnMat;
		}

		Scene scene;
		Node CameraNode;

		float Yaw { get; set; }
		float Pitch { get; set; }
		float Roll { get; set; }
		bool TouchEnabled { get; set; }
		const float TouchSensitivity = 2.5f;
		const float PixelSize = 0.01f;

		Text textInstrukce;
		Text textboxesCount;
		Timer nabijeni;
		bool nabito = true;

		List<Node> boxesNodesList = new List<Node> ();
		List<Node> ballNodesList = new List<Node> ();

		Vector3 cameraStartPosition = new Vector3 (0.0f, 5.0f, -20.0f);


		bool idle = false;
		int gameStateNow = 0;

		List<int> gameStatesForNewBoxes;
		List<int> buildListOfGameStates(int startState, int rate, int count) 
		{
			List<int> returnList = new List<int> ();
			int rateCount = 1;
			for (int i = startState; i <= count*rate+startState; i++) {
				if (rateCount==rate) {
					returnList.Add (i);
					rateCount = 1;
				} else {
					rateCount++;
				}

			}
			return returnList;

		}

		protected override void Start()
		{
			Input.SubscribeToKeyDown(e => { if (e.Key == Key.Esc) Engine.Exit(); });
						
			Graphics.WindowTitle = "PF 2016";

			nabijeni = new Timer (500);
			nabijeni.Elapsed += delegate {
				nabito = true;
			};

			base.Start();
			CreateScene();
			SetupViewport();

			initInstructionsText ("Ahoj"+Environment.NewLine+"Rozbi starou zed.");
			initBoxesCountText ("Zbývá "+boxesNodesList.Count+" kostiček.");

			initControls ();

		}

		int gameStateTemp = 0;
		bool destroyCheckOn = false;
		protected override void OnUpdate(float timeStep)
		{
			base.OnUpdate(timeStep);
			SimpleMoveCamera3D(timeStep);
			MoveCameraByTouches(timeStep);

			if (!idle) {
				gameStateNow++;
				if (gameStateNow == 1) {
					idle = true;
					destroyCheckOn = true;

				}  else if (gameStateNow == 100) {
					endCleanGarbageAction ();

				} else if (gameStateNow > 100 && gameStateNow < 200 ){
					moveCameraToFinal (cameraStartPosition,0,0);

				}  else if (gameStateNow == 200){
					endCleanGarbageAction ();
					removeCollision (boxesNodesList);
					boxesNodesList.Clear ();
					removeCollision (ballNodesList);
					ballNodesList.Clear ();
					gameStatesForNewBoxes = buildListOfGameStates(gameStateNow+1, 10, 15*8) ;

				} else if (gameStatesForNewBoxes!=null && gameStateNow >= gameStatesForNewBoxes[0] && gameStateNow <= gameStatesForNewBoxes[gameStatesForNewBoxes.Count-1]){
					if (gameStatesForNewBoxes.Contains (gameStateNow)) {
						createBoxAndAddToList (new Vector3 (0f, 50f, 0f));
					}

				} else if (gameStatesForNewBoxes!=null && gameStateNow==gameStatesForNewBoxes[gameStatesForNewBoxes.Count-1]+300) {
					removeCollision (boxesNodesList);
					resetRotation (boxesNodesList);
					buildNewWall ();
					gameStateTemp = gameStateNow;

				} else if (gameStateNow==gameStateTemp+1) {
					setCollision (boxesNodesList);

				} else if (gameStateNow==gameStateTemp+100) {
					KostickyActivity.oVibrator.Vibrate (KostickyActivity.vibrateSmall);
					colorBoxesBySchema (BoxColorSchemas.boxes2 (-5,2),matRandomizer());

				}  else if (gameStateNow==gameStateTemp+200) {
					KostickyActivity.oVibrator.Vibrate (KostickyActivity.vibrateSmall);
					colorBoxesBySchema (BoxColorSchemas.boxes0 (-1,2),matRandomizer());

				}  else if (gameStateNow==gameStateTemp+300) {
					KostickyActivity.oVibrator.Vibrate (KostickyActivity.vibrateSmall);
					colorBoxesBySchema (BoxColorSchemas.boxes1 (3,2),matRandomizer());

				}  else if (gameStateNow==gameStateTemp+400) {
					KostickyActivity.oVibrator.Vibrate (KostickyActivity.vibrateSmall);
					colorBoxesBySchema (BoxColorSchemas.boxes6 (5,2),matRandomizer());

				} else if (gameStateNow==gameStateTemp+500) {
					resetVariablesToStart ();

				} 

			}


			if (destroyCheckOn == true && boxesDestroyStateCheck ()) {
				idle = false;
				destroyCheckOn = false;
			}

		}

		void resetVariablesToStart()
		{
			getPitchedteSteps = false;
			getYawedteSteps = false;
			getMoveSteps = false;

			movedInPosition = false;
			yawedInPosition = false;
			pitchedInPosition = false;

			gameStateNow = 0;
		}

		int screenJoystickIndex;
		void initControls()
		{
			TouchEnabled = true;

			var layout = ResourceCache.GetXmlFile(Assets.UI.myJoystick);
			screenJoystickIndex = Input.AddScreenJoystick(layout, ResourceCache.GetXmlFile(Assets.UI.defaultStyle));
			Input.SetScreenJoystickVisible(screenJoystickIndex, true);
		}

		void hideMegaButton ()
		{

			Input.RemoveScreenJoystick (screenJoystickIndex);
	

			TouchEnabled = true;
			var layout = ResourceCache.GetXmlFile(Assets.UI.myJoystickNoMega);

			screenJoystickIndex = Input.AddScreenJoystick(layout, ResourceCache.GetXmlFile(Assets.UI.defaultStyle));
			Input.SetScreenJoystickVisible(screenJoystickIndex, true);

			Input.Update ();
		}

		void SimpleMoveCamera3D (float timeStep, float moveSpeed = 10.0f)
		{
			if (cameraMoving) {
				return;
			}

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
			if (Input.GetKeyPress (Key.Space)||Input.GetKeyPress (Key.M)) {
				textInstrukce.Value = "";
				if (nabito) {
					nabito = false;
					if (Input.GetKeyPress (Key.M)) {
						SpawnObject (true);
						hideMegaButton ();
					} else {
						SpawnObject (false);
					}

					nabijeni.Start ();

					if (textInstrukce.Value != null) {
						textInstrukce.Value = null;
					}

				}
			}

		}


		void MoveCameraByTouches (float timeStep)
		{
			if (cameraMoving) {
				return;
			}

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
			textInstrukce.SetFont(ResourceCache.GetFont(Assets.Fonts.Font), 20);
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
			textboxesCount.SetFont(ResourceCache.GetFont(Assets.Fonts.Font), 20);
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
			scene = new Scene();

			scene.CreateComponent<Octree>();
			scene.CreateComponent<PhysicsWorld>();
			scene.CreateComponent<DebugRenderer>();

			Node zoneNode = scene.CreateChild("Zone");
			Zone zone = zoneNode.CreateComponent<Zone>();
			zone.SetBoundingBox(new BoundingBox(-500.0f, 500.0f));

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
			skybox.Model=ResourceCache.GetModel(Assets.Models.box);
			skybox.SetMaterial(ResourceCache.GetMaterial("Materials/SkyboxSunSet.xml"));

			Node floorNode = scene.CreateChild("Floor");
			floorNode.Position=new Vector3(0.0f, -0.5f, 0.0f);
			floorNode.Scale=new Vector3(500.0f, 1.0f, 500.0f);
			StaticModel floorObject = floorNode.CreateComponent<StaticModel>();
			floorObject.Model=ResourceCache.GetModel(Assets.Models.box);
			floorObject.SetMaterial(ResourceCache.GetMaterial(Assets.Materials.terrain));
			floorNode.CreateComponent<RigidBody>(); 
			CollisionShape shape = floorNode.CreateComponent<CollisionShape>();
			shape.SetBox(Vector3.One, Vector3.Zero, Quaternion.Identity);

			for (int x = 0; x <= 14; x++) {
				for (int y = 0; y <= 7; y++) {
					createBoxAndAddToList (new Vector3 ((float)x + 0.3f - 6f, (float)y + 0.3f, 0));
				}
			}

			colorBoxesBySchema (BoxColorSchemas.boxes2 (-5, 2), matRandomizer());
			colorBoxesBySchema (BoxColorSchemas.boxes0 (-1, 2), matRandomizer());
			colorBoxesBySchema (BoxColorSchemas.boxes1 (3, 2), matRandomizer());
			colorBoxesBySchema (BoxColorSchemas.boxes5 (5, 2), matRandomizer());

			CameraNode = new Node();
			Camera oCamera = CameraNode.CreateComponent<Camera>();
			oCamera.FarClip = 500.0f;

			CameraNode.Position = cameraStartPosition;

		}

		public void createBoxAndAddToList(Vector3 position)
		{
			Node boxNode = scene.CreateChild("Box");
			boxNode.Position = position;
			StaticModel boxObject = boxNode.CreateComponent<StaticModel>();
			boxObject.Model=ResourceCache.GetModel(Assets.Models.box);
			boxObject.SetMaterial (ResourceCache.GetMaterial (Assets.Materials.gray));
			boxObject.CastShadows = true;
			RigidBody body = boxNode.CreateComponent<RigidBody>();
			body.Mass=1f;
			body.Friction=1f;
			CollisionShape shape = boxNode.CreateComponent<CollisionShape>();
			shape.SetBox(Vector3.One, Vector3.Zero, Quaternion.Identity);

			boxesNodesList.Add (boxNode);
		}

		void colorBoxesBySchema(List<int[]> schema, Material mat)
		{
			foreach (Node item in boxesNodesList) {
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


		void SpawnObject(bool mega)
		{

			var boxNode = scene.CreateChild("SmallBox");
			boxNode.Position = CameraNode.Position;
			boxNode.Rotation = CameraNode.Rotation;

			if (mega) {
				if (KostickyActivity.oVibrator!=null) {	KostickyActivity.oVibrator.Vibrate (KostickyActivity.vibrateBig);}
				boxNode.SetScale(3);
			} else {
				if (KostickyActivity.oVibrator!=null) {	KostickyActivity.oVibrator.Vibrate (KostickyActivity.vibrateSmall);}
				boxNode.SetScale(0.5f);
			}


			StaticModel boxModel = boxNode.CreateComponent<StaticModel>();
			boxModel.Model = ResourceCache.GetModel(Assets.Models.sphere);
			boxModel.SetMaterial(ResourceCache.GetMaterial(Assets.Materials.stone));
			boxModel.CastShadows = true;

			var body = boxNode.CreateComponent<RigidBody>();

			if (mega) {
				body.Mass = 10f;
			} else {
				body.Mass = 0.2f;
			}


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

		bool boxesDestroyStateCheck()
		{

			bool returnBool = true;
			int count = 0;
			foreach (Node item in boxesNodesList) {
				if (item.Position.Y>2) {
					count++;
					returnBool = false;

				} else {
					item.GetComponent<StaticModel> ().SetMaterial(ResourceCache.GetMaterial (Assets.Materials.orange));
				}
			}
			textboxesCount.Value =  ("Zbývá zbořit"+Environment.NewLine+count+" kostiček.");

			return returnBool;
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

		bool cameraMoving = false;

		void moveCameraToFinal(Vector3 targetPosition,float targetPitch, float targetYaw)
		{
			cameraMoving = true;

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
				idle = false;
				cameraMoving = false;
			}


		}

		void buildNewWall ()
		{

			int count = 0;
			for (int x = 0; x <= 14; x++) {
				for (int y = 0; y <= 7; y++) {
					if (count==boxesNodesList.Count) {
						return;
					}
					boxesNodesList[count].Position = new Vector3 ((float)x + 0.3f - 6f, (float)y + 0.5f, 0);
					count++;
				}
			}
		}

		void removeCollision(List <Node> nodesList)
		{
			foreach (Node item in nodesList) {
				item.RemoveComponent<CollisionShape> ();
			}
		}

		void setCollision(List <Node> nodesList)
		{
			foreach (Node item in nodesList) {
				CollisionShape shape = item.CreateComponent<CollisionShape>();
				shape.SetBox(Vector3.One, Vector3.Zero, Quaternion.Identity);
			}
		}

		void resetRotation(List <Node> nodesList)
		{
			foreach (Node item in nodesList) {
				item.Rotation = new Quaternion (0f, 0f, 0f);
			}
		}

	}
}

