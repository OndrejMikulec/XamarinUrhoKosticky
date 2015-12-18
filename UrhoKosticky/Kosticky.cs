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

	
		protected override void Start()
		{
			
			Graphics.SetWindowIcon(ResourceCache.GetImage ("Textures/UrhoIcon.png"));
			Graphics.WindowTitle = "PF 2016";

			nabijeni = new Timer (500);
			nabijeni.Elapsed += delegate {
				nabito = true;
			};

			base.Start();
			CreateScene();
			SetupViewport();

			initInstructionsText (@"Ahoj
Rozbi zed.");
			initBoxesCountText ("Zbývá "+boxesNodesList.Count+" kostiček.");

			initControls ();

		}


		protected override void OnUpdate(float timeStep)
		{
			base.OnUpdate(timeStep);
			SimpleMoveCamera3D(timeStep);
			MoveCameraByTouches(timeStep);
			destroyStateCheck ();

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


			List<int[]> boxes2 = new List<int[]> (){
				new int[] {1,1,0},
				new int[] {2,1,0},
				new int[] {1,2,0},
				new int[] {1,3,0},
				new int[] {2,3,0},
				new int[] {2,4,0},
				new int[] {2,5,0},
				new int[] {1,5,0},

				new int[] {1,1,1},
				new int[] {2,1,1},
				new int[] {1,2,1},
				new int[] {1,3,1},
				new int[] {2,3,1},
				new int[] {2,4,1},
				new int[] {2,5,1},
				new int[] {1,5,1}
			};

			List<int[]> boxes0 = new List<int[]> (){
				new int[] {4,1,0},
				new int[] {5,1,0},
				new int[] {6,1,0},
				new int[] {4,2,0},
				new int[] {6,2,0},
				new int[] {4,3,0},
				new int[] {6,3,0},
				new int[] {4,4,0},
				new int[] {6,4,0},
				new int[] {4,5,0},
				new int[] {5,5,0},
				new int[] {6,5,0},

				new int[] {4,1,1},
				new int[] {5,1,1},
				new int[] {6,1,1},
				new int[] {4,2,1},
				new int[] {6,2,1},
				new int[] {4,3,1},
				new int[] {6,3,1},
				new int[] {4,4,1},
				new int[] {6,4,1},
				new int[] {4,5,1},
				new int[] {5,5,1},
				new int[] {6,5,1},
			};

			List<int[]> boxes1 = new List<int[]> (){
				new int[] {8,1,0},
				new int[] {8,2,0},
				new int[] {8,3,0},
				new int[] {8,4,0},
				new int[] {8,5,0},

				new int[] {8,1,1},
				new int[] {8,2,1},
				new int[] {8,3,1},
				new int[] {8,4,1},
				new int[] {8,5,1},
			};

			List<int[]> boxes5 = new List<int[]> (){
				new int[] {10,1,0},
				new int[] {11,1,0},
				new int[] {11,2,0},
				new int[] {11,3,0},
				new int[] {10,3,0},
				new int[] {10,4,0},
				new int[] {10,5,0},
				new int[] {11,5,0},

				new int[] {10,1,1},
				new int[] {11,1,1},
				new int[] {11,2,1},
				new int[] {11,3,1},
				new int[] {10,3,1},
				new int[] {10,4,1},
				new int[] {10,5,1},
				new int[] {11,5,1}


			};


			for (int x = 0; x <= 12; x++) {
				for (int z = 0; z <= 0; z++) {
					for (int y = 0; y <= 6; y++) {
						Node boxNode = scene.CreateChild("Box");
						boxNode.Position=new Vector3((float)x+0.3f-6f,(float)y+0.3f,(float)z+0.3f);
						StaticModel boxObject = boxNode.CreateComponent<StaticModel>();
						boxObject.Model=cache.GetModel("Models/Box.mdl");

						if (vectorsListContains(boxes2,new int[]{x,y,z})) {
							boxObject.SetMaterial (cache.GetMaterial ("Materials/M_0020_Red.xml"));
						} 
						else if (vectorsListContains(boxes0,new int[]{x,y,z})) {
							boxObject.SetMaterial (cache.GetMaterial ("Materials/M_0060_GrassGreen.xml"));
						} else if (vectorsListContains(boxes1,new int[]{x,y,z})) {
							boxObject.SetMaterial (cache.GetMaterial ("Materials/M_0056_Yellow.xml"));
						} else if (vectorsListContains(boxes5,new int[]{x,y,z})) {
							boxObject.SetMaterial (cache.GetMaterial ("Materials/M_0103_Blue.xml"));
						} else {
							boxObject.SetMaterial (cache.GetMaterial ("Materials/M_0132_LightGray.xml"));
						}

						boxObject.CastShadows = true;

						RigidBody body = boxNode.CreateComponent<RigidBody>();
						body.Mass=1f;
						body.Friction=1f;
						CollisionShape shape = boxNode.CreateComponent<CollisionShape>();
						shape.SetBox(Vector3.One, Vector3.Zero, Quaternion.Identity);

						boxesNodesList.Add (boxNode);

					}
				}
			}


			CameraNode = new Node();
			Camera oCamera = CameraNode.CreateComponent<Camera>();
			oCamera.FarClip = 500.0f;


			CameraNode.Position = (new Vector3(0.0f, 5.0f, -20.0f));

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
			foreach (var item in boxesNodesList) {
				if (item.Position.Y>1) {
					count++;
					returnBool = false;

				} else {
					StringHash oStringHash = new StringHash (121334427);
					StaticModel oRigidBody = (StaticModel)item.GetComponent (oStringHash, false);
					oRigidBody.SetMaterial(cache.GetMaterial("Materials/M_0039_DarkOrange.xml"));					
				}
			}
			textboxesCount.Value =  (@"Zbývá zbořit"
+count+" kostiček.");

			return returnBool;
		}



	}
}

