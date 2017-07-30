
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MegaScatterObjectTexture))]
public class MegaScatterObjectTextureEditor : Editor
{
	private     MegaScatterObjectTexture	src;
	private     MegaUndo			undoManager;

	[MenuItem("GameObject/Create Other/MegaScatter/Scatter Object Texture")]
	static void CreateScatterObjectTexture()
	{
		Vector3 pos = Vector3.zero;

		if ( UnityEditor.SceneView.lastActiveSceneView != null )
			pos = UnityEditor.SceneView.lastActiveSceneView.pivot;

		GameObject go = new GameObject("Scatter Obj Texture");

		go.AddComponent<MegaScatterObjectTexture>();
		go.transform.position = pos;
		Selection.activeObject = go;
	}

	private void OnEnable()
	{
		src = target as MegaScatterObjectTexture;

		undoManager = new MegaUndo(src, "Mega Scatter Object Texture Param");
	}

	public override void OnInspectorGUI()
	{
		undoManager.CheckUndo();

#if !UNITY_5
		EditorGUIUtility.LookLikeControls();
#endif

		DisplayGUI();

		if ( GUI.changed )
			EditorUtility.SetDirty(target);

		undoManager.CheckDirty();
	}

	public void DisplayGUI()
	{
		MegaScatterObjectTexture mod = (MegaScatterObjectTexture)target;

		EditorGUILayout.LabelField("Objs: " + mod.objcount + " scattered: " + mod.scattercount);

		mod.seed = EditorGUILayout.IntField("Seed", mod.seed);
		mod.countmode = (MegaScatterMode)EditorGUILayout.EnumPopup("Count Mode", mod.countmode);
		mod.Density = EditorGUILayout.FloatField("Density", mod.Density);
		mod.forcecount = EditorGUILayout.IntField("Count", mod.forcecount);

		mod.globalScale = EditorGUILayout.Vector3Field("Global Scale", mod.globalScale);
		mod.scatterWidth = EditorGUILayout.FloatField("Width", mod.scatterWidth);
		mod.scatterLength = EditorGUILayout.FloatField("Length", mod.scatterLength);
		Texture2D scatterTexture = (Texture2D)EditorGUILayout.ObjectField("Texture", mod.scatterTexture, typeof(Texture2D), true);

		if ( scatterTexture != mod.scatterTexture )
		{
			if ( scatterTexture != null )
			{
				string texturePath = AssetDatabase.GetAssetPath(scatterTexture);
				TextureImporter textureImporter = (TextureImporter)AssetImporter.GetAtPath(texturePath);
				if ( textureImporter.isReadable )
					mod.scatterTexture = scatterTexture;
				else
					EditorUtility.DisplayDialog("Scatter Texture Select", "Texture is not set to Readable", "OK");
			}
			else
				mod.scatterTexture = scatterTexture;
		}

		mod.alphaDensity = EditorGUILayout.Toggle("Alpha Density", mod.alphaDensity);

		Texture2D scaleTexture = (Texture2D)EditorGUILayout.ObjectField("Scale", mod.scaleTexture, typeof(Texture2D), true);

		if ( scaleTexture != mod.scaleTexture )
		{
			if ( scaleTexture != null )
			{
				string texturePath = AssetDatabase.GetAssetPath(scaleTexture);
				TextureImporter textureImporter = (TextureImporter)AssetImporter.GetAtPath(texturePath);
				if ( textureImporter.isReadable )
					mod.scaleTexture = scaleTexture;
				else
					EditorUtility.DisplayDialog("Scale Texture Select", "Texture is not set to Readable", "OK");
			}
			else
				mod.scaleTexture = scaleTexture;
		}

		mod.mintexscale = EditorGUILayout.Slider("Min Scale", mod.mintexscale, 0.0f, 1.0f);
		mod.maxtexscale = EditorGUILayout.Slider("Max Scale", mod.maxtexscale, 0.0f, 1.0f);
		mod.minsizetoadd = EditorGUILayout.Slider("Min Size To Add", mod.minsizetoadd, 0.0f, 1.0f);

		mod.texturecollider = (Collider)EditorGUILayout.ObjectField("Texture Object", mod.texturecollider, typeof(Collider), true);

		mod.queryObject = (MegaScatterQuery)EditorGUILayout.ObjectField("Query Object", mod.queryObject, typeof(MegaScatterQuery), true);
		mod.dostaticbatching = EditorGUILayout.Toggle("Static Batching", mod.dostaticbatching);

		mod.raycast = EditorGUILayout.BeginToggleGroup("Raycast", mod.raycast);
		mod.NeedsGround = EditorGUILayout.Toggle("Needs Ground", mod.NeedsGround);
		mod.collisionOffset = EditorGUILayout.FloatField("Collision Offset", mod.collisionOffset);
		mod.showignoreobjs = EditorGUILayout.Foldout(mod.showignoreobjs, "Show Ignore Objects");

		if ( mod.showignoreobjs )
		{
			if ( GUILayout.Button("Add Ignore Obj") )
				mod.ignoreobjs.Add(new MegaScatterCollisionObj());

			EditorGUILayout.BeginVertical("box");
			EditorGUILayout.LabelField("Ignore Objects");
			for ( int i = 0; i < mod.ignoreobjs.Count; i++ )
			{
				EditorGUILayout.BeginHorizontal();
				GUILayout.Label("" + i + ":", GUILayout.MaxWidth(20));
				GUILayout.Label("On", GUILayout.MaxWidth(20));
				mod.ignoreobjs[i].active = EditorGUILayout.Toggle("", mod.ignoreobjs[i].active, GUILayout.MaxWidth(20));
				GUILayout.Label("Children", GUILayout.MaxWidth(52));
				mod.ignoreobjs[i].includechildren = EditorGUILayout.Toggle("", mod.ignoreobjs[i].includechildren, GUILayout.MaxWidth(20));
				mod.ignoreobjs[i].collider = (Collider)EditorGUILayout.ObjectField("", mod.ignoreobjs[i].collider, typeof(Collider), true, GUILayout.MaxWidth(180));

				//mod.ignoreobjs[i] = (Collider)EditorGUILayout.ObjectField("" + i + ":", mod.ignoreobjs[i], typeof(Collider), true);
				if ( GUILayout.Button("Delete", GUILayout.MaxWidth(50)) )
					mod.ignoreobjs.RemoveAt(i);

				EditorGUILayout.EndHorizontal();
			}

			EditorGUILayout.EndVertical();
		}

		if ( mod.raycast )
		{
			if ( mod.surfaces.Count == 0 )
				EditorGUILayout.HelpBox("No Surface Objects Defined! No Objects will be scattered", MessageType.Info, true);
		}

		mod.showsurfaceobjs = EditorGUILayout.Foldout(mod.showsurfaceobjs, "Show Surface Objects");
		if ( mod.showsurfaceobjs )
		{
			if ( GUILayout.Button("Add Surface Obj") )
				mod.surfaces.Add(new MegaScatterCollisionObj());

			EditorGUILayout.BeginVertical("box");
			EditorGUILayout.LabelField("Surface Objects");
			for ( int i = 0; i < mod.surfaces.Count; i++ )
			{
				EditorGUILayout.BeginHorizontal();
				GUILayout.Label("" + i + ":");
				mod.surfaces[i].active = EditorGUILayout.Toggle("", mod.surfaces[i].active, GUILayout.MaxWidth(20));
				mod.surfaces[i].collider = (Collider)EditorGUILayout.ObjectField("", mod.surfaces[i].collider, typeof(Collider), true);

				mod.surfaces[i].prebuildenable = EditorGUILayout.Toggle("", mod.surfaces[i].prebuildenable, GUILayout.MaxWidth(20));
				mod.surfaces[i].postbuilddisable = EditorGUILayout.Toggle("", mod.surfaces[i].postbuilddisable, GUILayout.MaxWidth(20));

				//mod.ignoreobjs[i] = (Collider)EditorGUILayout.ObjectField("" + i + ":", mod.ignoreobjs[i], typeof(Collider), true);
				if ( GUILayout.Button("Delete", GUILayout.MaxWidth(50)) )
					mod.surfaces.RemoveAt(i);

				EditorGUILayout.EndHorizontal();
			}

			EditorGUILayout.EndVertical();
		}

		EditorGUILayout.EndToggleGroup();

		mod.hideObjects = EditorGUILayout.Toggle("Hide Objects", mod.hideObjects);
		mod.buildOnStart = EditorGUILayout.Toggle("Build on Start", mod.buildOnStart);
		mod.useCoRoutine = EditorGUILayout.Toggle("Scatter Over Frames", mod.useCoRoutine);
		mod.NumPerFrame = EditorGUILayout.IntField("Number Per Frame", mod.NumPerFrame);
		mod.displaygizmo = EditorGUILayout.Toggle("Display Gizmo", mod.displaygizmo);

		EditorGUILayout.BeginHorizontal();
		if ( GUILayout.Button("Hide/Show Objects") )
			mod.HideShow();

		if ( GUILayout.Button("Remove Objects") )
			mod.RemoveObjects();

		if ( GUILayout.Button("Add Mesh") )
		{
			MegaScatterLayer inf = new MegaScatterLayer();
			mod.layers.Add(inf);
			mod.currentEdit = mod.layers.Count - 1;
		}
		EditorGUILayout.EndHorizontal();

		mod.showadvanced = EditorGUILayout.Foldout(mod.showadvanced, "Advanced");
		if ( mod.showadvanced )
		{
			mod.FailCount = EditorGUILayout.IntField("Fail Count", mod.FailCount);
			mod.PosFailCount = EditorGUILayout.IntField("Pos Fail Count", mod.PosFailCount);
		}

		EditorGUILayout.BeginVertical("box");
		EditorGUILayout.LabelField("Layers");
		for ( int i = 0; i < mod.layers.Count; i++ )
		{
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("" + i + " - " + mod.layers[i].LayerName);
			mod.layers[i].Enabled = EditorGUILayout.Toggle("", mod.layers[i].Enabled, GUILayout.MaxWidth(20));

			if ( GUILayout.Button("Edit", GUILayout.MaxWidth(50)) )
			{
				mod.currentEdit = i;
				EditorUtility.SetDirty(mod);
			}

			if ( GUILayout.Button("Delete", GUILayout.MaxWidth(50)) )
				mod.layers.RemoveAt(i);

			if ( GUILayout.Button("U", GUILayout.MaxWidth(20)) )
			{
				if ( i > 0 )
					MegaScatterMeshEditor.SwapInf(mod, i, i - 1);
			}

			if ( GUILayout.Button("D", GUILayout.MaxWidth(22)) )
			{
				if ( i < mod.layers.Count - 1 )
					MegaScatterMeshEditor.SwapInf(mod, i, i + 1);
			}

			EditorGUILayout.EndHorizontal();
		}
		EditorGUILayout.EndVertical();

		GUI.backgroundColor = Color.green;
		if ( GUILayout.Button("Update", GUILayout.Height(30)) )
		{
			mod.update = true;
			EditorUtility.SetDirty(target);
		}
		GUI.backgroundColor = Color.white;

		if ( mod.currentEdit >= mod.layers.Count )
			mod.currentEdit = mod.layers.Count - 1;

		int ci = mod.currentEdit;

		if ( ci >= 0 )
		{
			MegaScatterLayer inf = mod.layers[ci];

			EditorGUILayout.BeginVertical("box");

			EditorGUILayout.BeginHorizontal();
			if ( GUILayout.Button("Copy", GUILayout.Width(50)) )
				MegaScatterLayer.copylayer = inf;

			if ( MegaScatterLayer.copylayer != null )
			{
				if ( GUILayout.Button("Paste from " + MegaScatterLayer.copylayer.LayerName) )
					inf.Copy(MegaScatterLayer.copylayer);
			}
			EditorGUILayout.EndHorizontal();

			inf.LayerName = EditorGUILayout.TextField("Name", inf.LayerName);
			inf.Enabled = EditorGUILayout.BeginToggleGroup("Enabled", inf.Enabled);
			inf.markstatic = EditorGUILayout.Toggle("Mark Static", inf.markstatic);

			EditorGUILayout.BeginVertical("box");
			if ( GUILayout.Button("Add Color Mask") )
			{
				inf.scattercols.Add(new MegaScatterCol());
			}

			for ( int cv = 0; cv < inf.scattercols.Count; cv++ )
			{
				EditorGUILayout.BeginHorizontal();
				GUILayout.Label("Mask Col " + cv + ":");
				inf.scattercols[cv].lowcol = EditorGUILayout.ColorField("", inf.scattercols[cv].lowcol, GUILayout.MaxWidth(50));
				inf.scattercols[cv].highcol = EditorGUILayout.ColorField("", inf.scattercols[cv].highcol, GUILayout.MaxWidth(50));

				if ( GUILayout.Button("Delete", GUILayout.MaxWidth(50)) )
					inf.scattercols.RemoveAt(cv);

				EditorGUILayout.EndHorizontal();
			}
			EditorGUILayout.EndVertical();

			inf.obj = (GameObject)EditorGUILayout.ObjectField("Object", inf.obj, typeof(GameObject), true);
			inf.forcecount = EditorGUILayout.IntField("Force Count", inf.forcecount);
			inf.maxcount = EditorGUILayout.IntField("Max Count", inf.maxcount);
			inf.weight = EditorGUILayout.FloatField("Weight", inf.weight);
			inf.scale = EditorGUILayout.FloatField("Scale", inf.scale);
			inf.offsetLow = EditorGUILayout.Vector3Field("Offset Low", inf.offsetLow);
			inf.offsetHigh = EditorGUILayout.Vector3Field("Offset High", inf.offsetHigh);
			inf.prerot = EditorGUILayout.Vector3Field("Pre Rot", inf.prerot);
			inf.rotLow = EditorGUILayout.Vector3Field("Rot Low", inf.rotLow);
			inf.rotHigh = EditorGUILayout.Vector3Field("Rot High", inf.rotHigh);
			inf.uniformScaling = EditorGUILayout.Toggle("Uniform Scaling", inf.uniformScaling);

			if ( inf.uniformScaling )
			{
				inf.uniscalemode = (MegaScatterScaleMode)EditorGUILayout.EnumPopup("Mode", inf.uniscalemode);
				inf.uniscaleLow = EditorGUILayout.FloatField("Scale Low", inf.uniscaleLow);
				inf.uniscaleHigh = EditorGUILayout.FloatField("Scale High", inf.uniscaleHigh);
			}
			else
			{
				inf.scaleLow = EditorGUILayout.Vector3Field("Scale Low", inf.scaleLow);
				inf.scaleHigh = EditorGUILayout.Vector3Field("Scale High", inf.scaleHigh);
			}

			inf.snap = EditorGUILayout.Vector3Field("Snap", inf.snap);
			inf.snapRot = EditorGUILayout.Vector3Field("Snap Rot", inf.snapRot);
			inf.distCrv = EditorGUILayout.CurveField("Dist Curve", inf.distCrv);
			inf.seed = EditorGUILayout.IntField("Seed", inf.seed);
			inf.noOverlap = EditorGUILayout.Toggle("No Overlap", inf.noOverlap);
			inf.clearOverlap = EditorGUILayout.Toggle("Clear Overlap", inf.clearOverlap);
			inf.radius = EditorGUILayout.FloatField("Radius", inf.radius);
			inf.colradiusadj = EditorGUILayout.FloatField("Col Radius Adj", inf.colradiusadj);
			inf.raycount = EditorGUILayout.IntSlider("Ray Count", inf.raycount, 1, 8);
			inf.align = EditorGUILayout.Slider("Align", inf.align, 0.0f, 1.0f);

			inf.useheight = EditorGUILayout.BeginToggleGroup("Use Height Limits", inf.useheight);
			inf.minheight = EditorGUILayout.FloatField("Min Height", inf.minheight);
			inf.maxheight = EditorGUILayout.FloatField("Max Height", inf.maxheight);
			EditorGUILayout.EndToggleGroup();

			inf.showcolvari = EditorGUILayout.Foldout(inf.showcolvari, "Color Variations");

			if ( inf.showcolvari )
			{
				EditorGUILayout.BeginVertical("box");
				Texture2D colorTexture = (Texture2D)EditorGUILayout.ObjectField("Color", inf.colorTexture, typeof(Texture2D), true);

				if ( colorTexture != inf.colorTexture )
				{
					if ( colorTexture != null )
					{
						string texturePath = AssetDatabase.GetAssetPath(colorTexture);
						TextureImporter textureImporter = (TextureImporter)AssetImporter.GetAtPath(texturePath);
						if ( textureImporter.isReadable )
							inf.colorTexture = colorTexture;
						else
							EditorUtility.DisplayDialog("Color Texture Select", "Texture is not set to Readable", "OK");
					}
					else
						inf.colorTexture = colorTexture;
				}

				if ( GUILayout.Button("Add Color") )
				{
					inf.colvariations.Add(Color.white);
				}

				for ( int cv = 0; cv < inf.colvariations.Count; cv++ )
				{
					EditorGUILayout.BeginHorizontal();
					inf.colvariations[cv] = EditorGUILayout.ColorField("" + cv + ":", inf.colvariations[cv]);

					if ( GUILayout.Button("Delete", GUILayout.MaxWidth(50)) )
						inf.colvariations.RemoveAt(cv);

					EditorGUILayout.EndHorizontal();
				}
				EditorGUILayout.EndVertical();
			}

			inf.disableCollider = EditorGUILayout.Toggle("Disable Collider", inf.disableCollider);

			EditorGUILayout.EndToggleGroup();

			if ( GUILayout.Button("Delete") )
			{
				mod.layers.RemoveAt(ci);
				mod.currentEdit = 0;
			}

			EditorGUILayout.EndVertical();

			GUI.backgroundColor = Color.green;
			if ( GUILayout.Button("Update", GUILayout.Height(30)) )
			{
				mod.update = true;
				EditorUtility.SetDirty(target);
			}
			GUI.backgroundColor = Color.white;
		}
	}

	void OnSceneGUI()
	{
		MegaScatterObjectTexture mod = (MegaScatterObjectTexture)target;

		MegaScatterMeshEditor.DisplayGizmo(mod, mod.transform.localToWorldMatrix);

		Handles.matrix = mod.transform.localToWorldMatrix;

		Handles.color = Color.green;

		Vector3 p1 = Vector3.zero;
		Vector3 p2 = Vector3.zero;
		Vector3 p3 = Vector3.zero;
		Vector3 p4 = Vector3.zero;

		p1.x = -mod.scatterWidth * 0.5f;
		p1.z = -mod.scatterLength * 0.5f;

		p2.x = mod.scatterWidth * 0.5f;
		p2.z = -mod.scatterLength * 0.5f;

		p3.x = mod.scatterWidth * 0.5f;
		p3.z = mod.scatterLength * 0.5f;

		p4.x = -mod.scatterWidth * 0.5f;
		p4.z = mod.scatterLength * 0.5f;
		Handles.DrawLine(p1, p2);
		Handles.DrawLine(p2, p3);
		Handles.DrawLine(p3, p4);
		Handles.DrawLine(p4, p1);
	}
}
