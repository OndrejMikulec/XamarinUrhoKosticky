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

		Scene myScene;
		Node myCameraNode;
		Skybox mySkybox;
		Light myLightDay;


		float Yaw { get; set; }
		float Pitch { get; set; }
		float Roll { get; set; }
		bool TouchEnabled { get; set; }
		const float TouchSensitivity = 2.5f;
		const float PixelSize = 0.01f;

		Text screenText;
		Text textboxesCount;

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
						
			Graphics.WindowTitle = "Happy New Year 2016";

			base.Start();
			CreateScene();
			SetupViewport();

			initInstructionsText ("Hello!"+Environment.NewLine+"Destroy the old wall!");
			initBoxesCountText (boxesNodesList.Count+" boxes left.");

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

				} else if (gameStateNow > 100 && gameStateNow < 350 ){
					cameraMoving = true;

					moveCameraToFinal (cameraStartPosition,0,0);

				}  else if (gameStateNow == 350){
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

					settingNight();

				} else if (gameStatesForNewBoxes!=null && gameStateNow==gameStatesForNewBoxes[gameStatesForNewBoxes.Count-1]+300) {
					removeCollision (boxesNodesList);
					resetRotation (boxesNodesList);
					buildNewWall ();
					gameStateTemp = gameStateNow;

				} else if (gameStateNow==gameStateTemp+1) {
					setCollision (boxesNodesList);
					seConstructionLight ();
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

					setFinalLights ();

				} 

			}


			if (destroyCheckOn == true && boxesDestroyStateCheck ()) {
				idle = false;
				destroyCheckOn = false;
				textboxesCount.Value =  null;
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

		int screenJoystick1Index = 0;
		int screenJoystick2Index = 0;
		void initControls()
		{
			TouchEnabled = true;

			var layout = ResourceCache.GetXmlFile(Assets.UI.myJoystick);
			screenJoystick1Index = Input.AddScreenJoystick(layout, ResourceCache.GetXmlFile(Assets.UI.defaultStyle));
			Input.SetScreenJoystickVisible(screenJoystick1Index, true);
		}

		//alternative solution
		//screen joysticks swap
		//KeyDown event froze in action -> joysticks needs different keys
		void hideMegaButton ()
		{
			Input.RemoveScreenJoystick (screenJoystick1Index);


			TouchEnabled = true;
			var layout = ResourceCache.GetXmlFile(Assets.UI.myJoystickNoMega);

			screenJoystick2Index = Input.AddScreenJoystick(layout, ResourceCache.GetXmlFile(Assets.UI.defaultStyle));
			Input.SetScreenJoystickVisible(screenJoystick1Index, true);

		}

		void SimpleMoveCamera3D (float timeStep, float moveSpeed = 10.0f)
		{
			

			if (!TouchEnabled) {
				return;
			}

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

			myCameraNode.Rotation = new Quaternion(Pitch, Yaw, 0);


			if (Input.GetKeyDown (Key.W)&&screenJoystick2Index == 0) myCameraNode.Translate ( Vector3.UnitZ * moveSpeed * timeStep);
			if (Input.GetKeyDown (Key.S)&&screenJoystick2Index == 0) myCameraNode.Translate (-Vector3.UnitZ * moveSpeed * timeStep);
			if (Input.GetKeyDown (Key.A)&&screenJoystick2Index == 0) myCameraNode.Translate (-Vector3.UnitX * moveSpeed * timeStep);
			if (Input.GetKeyDown (Key.D)&&screenJoystick2Index == 0) myCameraNode.Translate ( Vector3.UnitX * moveSpeed * timeStep);

			if (Input.GetKeyDown (Key.T)) myCameraNode.Translate ( Vector3.UnitZ * moveSpeed * timeStep);
			if (Input.GetKeyDown (Key.G)) myCameraNode.Translate (-Vector3.UnitZ * moveSpeed * timeStep);
			if (Input.GetKeyDown (Key.F)) myCameraNode.Translate (-Vector3.UnitX * moveSpeed * timeStep);
			if (Input.GetKeyDown (Key.H)) myCameraNode.Translate ( Vector3.UnitX * moveSpeed * timeStep);

			if (Input.GetKeyPress (Key.Space)||Input.GetKeyPress (Key.M)) {
				screenText.Value = "";

				if (Input.GetKeyPress (Key.M)) {
					SpawnObject (true);
					hideMegaButton ();
				} else {
					SpawnObject (false);
				}

				if (screenText.Value != null) {
					screenText.Value = null;
				}
			}

		}


		void MoveCameraByTouches (float timeStep)
		{

			if (!TouchEnabled) {
				return;
			}

			if (cameraMoving) {
				return;
			}

			if (!TouchEnabled || myCameraNode == null)
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
					var camera = myCameraNode.GetComponent<Camera>();
					if (camera == null)
						return;

					var graphics = Graphics;
					Yaw += TouchSensitivity * camera.Fov / graphics.Height * state.Delta.X;
					Pitch += TouchSensitivity * camera.Fov / graphics.Height * state.Delta.Y;
					myCameraNode.Rotation = new Quaternion(Pitch, Yaw, 0);

				}

			}
		}


		void initInstructionsText(string text = "")
		{
			screenText = new Text()
			{
				Value = text,
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center
			};
			screenText.SetFont(ResourceCache.GetFont(Assets.Fonts.Font), 20);
			screenText.SetColor(new Color(0f, 1f, 0f));
			UI.Root.AddChild(screenText);
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
			renderer.SetViewport(0, new Viewport(Context, myScene, myCameraNode.GetComponent<Camera>(), null));
		}

		void CreateScene()
		{
			myScene = new Scene();

			myScene.CreateComponent<Octree>();
			myScene.CreateComponent<PhysicsWorld>();
			myScene.CreateComponent<DebugRenderer>();

			Node zoneNode = myScene.CreateChild("Zone");
			Zone zone = zoneNode.CreateComponent<Zone>();
			zone.SetBoundingBox(new BoundingBox(-500.0f, 500.0f));

			Node lightNode = myScene.CreateChild("DirectionalLight");
			lightNode.SetDirection(new Vector3(0.6f, -1.0f, 0.8f));
			myLightDay = lightNode.CreateComponent<Light>();
			myLightDay.LightType=LightType.Directional;
			myLightDay.CastShadows=true;
			myLightDay.ShadowBias=new BiasParameters(0.00025f, 0.5f);
			myLightDay.ShadowCascade=new CascadeParameters(10.0f, 50.0f, 200.0f, 0.0f, 0.8f);


			Node skyNode = myScene.CreateChild("Sky");
			skyNode.SetScale(500.0f); // The scale actually does not matter
			mySkybox = skyNode.CreateComponent<Skybox>();
			mySkybox.Model=ResourceCache.GetModel(Assets.Models.box);
			mySkybox.SetMaterial(ResourceCache.GetMaterial(Assets.Materials.skyBoxSunSet));

			Node floorNode = myScene.CreateChild("Floor");
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

			myCameraNode = new Node();
			Camera oCamera = myCameraNode.CreateComponent<Camera>();
			oCamera.FarClip = 500.0f;

			myCameraNode.Position = cameraStartPosition;

		}

		public void createBoxAndAddToList(Vector3 position)
		{
			Node boxNode = myScene.CreateChild("Box");
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

			var boxNode = myScene.CreateChild("SmallBox");
			boxNode.Position = myCameraNode.Position;
			boxNode.Rotation = myCameraNode.Rotation;

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

			body.SetLinearVelocity(myCameraNode.Rotation * new Vector3(0f, 0.25f, 1f) * objectVelocity);

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
			textboxesCount.Value =  (Environment.NewLine+count+" boxes left.");

			return returnBool;
		}

		void endCleanGarbageAction()
		{
			Random dir = new Random ();

			foreach (Node item in boxesNodesList) {
				endCleaning (item,dir);
			}

			foreach (Node item in ballNodesList) {
				endCleaning (item,dir);
			}				
		}

		void endCleaning(Node item,Random dir )
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
			

			if (!pitchedInPosition) {
				if (!getPitchedteSteps) {
					getPitchedteSteps = true;

					stepPitch = ( targetPitch - myCameraNode.Rotation.PitchAngle )/10; 
				}

				if (Math.Round(myCameraNode.Rotation.PitchAngle,1)!=Math.Round(targetPitch,1)	) {
					Pitch += stepPitch;

					myCameraNode.Rotation = new Quaternion (Pitch,0,0);

				} else {
					pitchedInPosition = true;
				}				
			}

			if (pitchedInPosition&&!yawedInPosition) {
				if (!getYawedteSteps) {
					getYawedteSteps = true;

					stepYaw = ( myCameraNode.Rotation.YawAngle -targetYaw)/10; 

				}

				if (	Math.Round(myCameraNode.Rotation.YawAngle,1)!=Math.Round(targetYaw,1)	) {
					Yaw += stepYaw;

					myCameraNode.Rotation = new Quaternion (0,Yaw,0);

				} else {
					yawedInPosition = true;
				}				
			}

			if (pitchedInPosition&&yawedInPosition&&!movedInPosition) {
				if (!getMoveSteps) {
					getMoveSteps = true;
					stepX = ( targetPosition.X - myCameraNode.Position.X)/100; 
					stepY = ( targetPosition.Y - myCameraNode.Position.Y)/100; 
					stepZ = ( targetPosition.Z - myCameraNode.Position.Z)/100; 

				}

				if (	Math.Round(targetPosition.X,1)!=Math.Round( myCameraNode.Position.X,1)
					|| 	Math.Round(targetPosition.Y,1)!=Math.Round( myCameraNode.Position.Y,1)
					|| 	Math.Round(targetPosition.Z,1)!=Math.Round( myCameraNode.Position.Z,1)
				) {

				float cameraPositionX = myCameraNode.Position.X + stepX;
					float cameraPositionY = myCameraNode.Position.Y + stepY;
					float cameraPositionZ = myCameraNode.Position.Z + stepZ;

				myCameraNode.Position = new Vector3(cameraPositionX,cameraPositionY,cameraPositionZ);
				} else {
					movedInPosition = true;
				}			
			}

			if (	movedInPosition
				&& 	yawedInPosition
				&& 	pitchedInPosition) {
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

		void settingNight()
		{
			myLightDay.Brightness -= 0.001f;
		}

		Light lightConst;
		void seConstructionLight()
		{
			Node lightNode = myScene.CreateChild("PointLight");
			lightConst = lightNode.CreateComponent<Light>();
			lightConst.LightType = LightType.Point;
			lightConst.Range = (10.0f);
			lightNode.Position  = new Vector3 (0.0f, 5.0f, -2.0f);
		}

		void setFinalLights()
		{
			myLightDay.Enabled = false;
			lightConst.Enabled = false;

			Node lightNode = myScene.CreateChild("PointLight");
			Light light = lightNode.CreateComponent<Light>();
			light.LightType = LightType.Point;
			light.Range = (10.0f);

			ObjectAnimation lightAnimation=new ObjectAnimation();

			ValueAnimation positionAnimation=new ValueAnimation();
			positionAnimation.InterpolationMethod= InterpMethod.Spline;
			positionAnimation.SplineTension=0.7f;

			positionAnimation.SetKeyFrame(0.0f, new Vector3(-5.0f, 5.0f, -3.0f));
			positionAnimation.SetKeyFrame(1.0f, new Vector3(5.0f, 5.0f, -3.0f));
			positionAnimation.SetKeyFrame(2.0f, new Vector3(5.0f, 5.0f, -2.0f));
			positionAnimation.SetKeyFrame(3.0f, new Vector3(-5.0f, 5.0f, -2.0f));
			positionAnimation.SetKeyFrame(4.0f, new Vector3(-5.0f, 5.0f, -3.0f));

			lightAnimation.AddAttributeAnimation("Position", positionAnimation, WrapMode.Loop, 1f);

			ValueAnimation colorAnimation=new ValueAnimation();
			colorAnimation.SetKeyFrame(0.0f, Color.White);
			colorAnimation.SetKeyFrame(1.0f, Color.Red);
			colorAnimation.SetKeyFrame(2.0f, Color.Yellow);
			colorAnimation.SetKeyFrame(3.0f, Color.Green);
			colorAnimation.SetKeyFrame(4.0f, Color.Blue);

			colorAnimation.SetKeyFrame(5.0f, Color.Magenta);
			colorAnimation.SetKeyFrame(6.0f, Color.Cyan);
			colorAnimation.SetKeyFrame(7.0f, Color.Red);
			colorAnimation.SetKeyFrame(8.0f, Color.White);

			lightAnimation.AddAttributeAnimation("@Light/Color", colorAnimation, WrapMode.Loop, 1f);
			lightNode.ObjectAnimation=lightAnimation;
		}

	}
}

