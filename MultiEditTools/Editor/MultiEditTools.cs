using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.Linq;

namespace MultiEditToolsNs
{
    /// <summary>
    /// Multi Edit Tools. By Stanton Oosthuizen. November 2017.    
    /// </summary>
    
#if UNITY_EDITOR

    #region ToolsWindow
    public class MultiEditTools : EditorWindow
    {
       
        #region Variables

        /// <summary>
        /// These are the various variables used within this script.
        /// </summary>

        //Various variables
        private const int SelectionThresholdForProgressBar = 20;
        Vector2 scrollPos;
        private delegate void ChangePrefab(GameObject go);

        //Menu Fold Outs
        private static bool searchFold = false;
        private static bool renameFold = false;
        private static bool rigidbodyFold = false;
        private static bool collidersFold = false;
        private static bool renderersFold = false;
        private static bool prefabsFold = false;
        private static bool scriptsFold = false;
        private static bool componentFold = false;

        private static string consoleOutput;
        private int colliderMode = 0;

        //Search variables
        private static bool useSearch = false;
        private static string searchString;
        private static bool searchCaseSentive = false;
        private static bool searchTagCaseSentive = false;
        private static Int32 searchLayer = 0;
        private static string searchTag = "Untagged";

        //Rename sariables
        private static bool replaceString = false;
        private static string replaceThis; 
        private static bool partialCaseSentive = false;

        //New Name
        private static string withThis; 

        //Enum Variables
        private static bool useEnum = false;
        private static bool prefixInstead = false;
        private static int startEnum = 1;

        //Prefabs Variables
        private static string newPrefabsLocation = "Assets/Prefabs";

        //Include Children Variables
        private static bool includeChildrenColliders = false;
        private static bool includeChildrenRenderers = false;
        private static bool includeChildrenRigidbodies = false;
        private static bool includeChildrenRename = false;
        private static bool includeChildrenScripts = false;
        private static bool includeChildrenComponents = false;


        //Attach Script 
        private static MonoScript scriptToAdd = null;
        private static bool addScript = false;
        private static MonoScript scriptToRemove = null;
        private static bool removeScript = false;        

        //Component Variables
        private GameObject newGameobject = null;
        private List<GameObject> createdObjects = new List<GameObject>();
        private static bool compAdded = false;
        private static bool nonTransformCompFound = false;
        private static bool addComp = false;
        private static bool removeComp = false;
        private static bool updateComp = true;        
        private List<Editor> myEditors = new List<Editor>();

        //Search options
        private static SEARCHOPTIONS op;
        private enum SEARCHOPTIONS
        {
            CONTAINS = 0,
            STARTS_WITH = 1,
            ENDS_WITH = 2
        }

        private static SEARCHTYPE sType;
        private enum SEARCHTYPE
        {
            NAME = 0,
            TAG = 1,
            LAYER = 2
        }


        #endregion        

        #region Menu

        /// <summary>
        /// These are the functions that create the menu entry and show the window.
        /// </summary>        

        [MenuItem("Window/Multi Edit Tools", true, -100)]
        static bool ValidateRenameObjects()
        {
            //removing this for simplicity
            //return Selection.activeGameObject != null;                
            return true;
        }

        [MenuItem("Window/Multi Edit Tools")]
        static void Init()
        {
            MultiEditTools window = CreateInstance<MultiEditTools>();
            window.Show();
            window.titleContent.text = "MultiEditTools";
            window.position = new Rect(20,150, 400, 500);           
        }


        #endregion

        #region GUI

        /// <summary>
        /// This is the main GUI of the editor window.
        /// </summary>
            

        private void OnDisable()
        {
            ClearComponents();
            RemoveHelpers();
        }

        private void OnDestroy()
        {
            ClearComponents();
            RemoveHelpers();
        }

        void OnGUI()
        {
            try
            {                               
                //Make the Foldout headings appear in Bold text
                GUIStyle myFoldoutStyle = new GUIStyle(EditorStyles.foldout)
                {
                    fontStyle = FontStyle.Bold
                };

                EditorGUILayout.BeginVertical();
                scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

                //Search Foldout
                searchFold = EditorGUILayout.Foldout(searchFold, new GUIContent("Search Tool", "Search for specific objects within the selected objects"), myFoldoutStyle);
                if (searchFold)
                {
                    EditorGUILayout.BeginVertical(GUI.skin.box);
                    EditorGUILayout.Space();
                        
                    //Filter selection via a name search
                    useSearch = EditorGUILayout.Toggle(new GUIContent("Search", "Only change objects that match a specific pattern"), useSearch);
                    EditorGUI.BeginDisabledGroup(useSearch == false);

                    EditorGUILayout.Space();
                    EditorGUILayout.Space();

                    sType = (SEARCHTYPE)EditorGUILayout.EnumPopup(new GUIContent("Search Type", "How would you like you search"), sType);

                    EditorGUILayout.Space();
                    EditorGUILayout.Space();

                    if(sType == SEARCHTYPE.NAME)
                    {
                        searchString = EditorGUILayout.TextField(new GUIContent("Name Contains", "The pattern to search for in the names of the selected objects"), searchString);
                        op = (SEARCHOPTIONS)EditorGUILayout.EnumPopup(new GUIContent("Search Method", "Where to search in the names of the selected objects"), op);
                        searchCaseSentive = EditorGUILayout.Toggle(new GUIContent("Case sensitive", "Must the case match exactly"), searchCaseSentive);
                    }
                    else if(sType == SEARCHTYPE.TAG)
                    {
                        searchTag = EditorGUILayout.TagField(new GUIContent("Select Tag", "The object must have the following tag"), searchTag);
                        searchTagCaseSentive = EditorGUILayout.Toggle(new GUIContent("Case sensitive", "Must the case match exactly"), searchTagCaseSentive);
                    }
                    else if(sType == SEARCHTYPE.LAYER)
                    {
                        searchLayer = EditorGUILayout.LayerField(new GUIContent("Select Layer", "The object must be on the following layer"), searchLayer);
                    }
                                               
                    EditorGUI.EndDisabledGroup();

                    EditorGUILayout.Space();
                    EditorGUILayout.Space();
                    EditorGUILayout.EndVertical();
                }

                //Prefabs Foldout
                prefabsFold = EditorGUILayout.Foldout(prefabsFold, new GUIContent("Prefabs Tool", "Create/Update Multiple Prefabs"), myFoldoutStyle);
                if (prefabsFold)
                {
                    EditorGUILayout.BeginVertical(GUI.skin.box);
                    EditorGUILayout.Space();
                    //The directory in which the new prefabs will be created
                    newPrefabsLocation = EditorGUILayout.TextField(new GUIContent("Prefabs Folder", "The directory in which new prefabs will be created"), newPrefabsLocation);

                    EditorGUILayout.Space();

                    //Create Prefabs                            
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();                        
                    if (GUILayout.Button("Create Prefab(s)", GUILayout.Width(160), GUILayout.Height(20)))
                    {                            
                        CreatePrefab();
                    }
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.Space();

                    //Apply and Revert Changes
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button("Apply Changes", GUILayout.Width(160), GUILayout.Height(20)))
                        {
                            ApplyPrefabs();
                        }

                        GUILayout.FlexibleSpace();

                        if (GUILayout.Button("Revert Changes", GUILayout.Width(160), GUILayout.Height(20)))
                        {
                            ResetPrefabs();
                        }
                        GUILayout.FlexibleSpace();
                    }
                    GUILayout.EndHorizontal();
                    EditorGUILayout.Space();
                    EditorGUILayout.Space();
                    EditorGUILayout.EndVertical();
                }

                //Rename Foldout                
                renameFold = EditorGUILayout.Foldout(renameFold, new GUIContent("Rename Tool", "Rename objects partially or completely"), myFoldoutStyle);
                if (renameFold)
                {
                    EditorGUILayout.BeginVertical(GUI.skin.box);
                    EditorGUILayout.Space();
                        
                    //The new name or partial name for the files that are selected (and possibly filtered via a search)
                    withThis = EditorGUILayout.TextField(new GUIContent("New Name:", "The new name or partial name"), withThis);
                    includeChildrenRename = EditorGUILayout.Toggle(new GUIContent("Include Children", "Also rename matching child objects (can be filtered via the search)"), includeChildrenRename);
                    EditorGUILayout.Space();

                    //Use an enumerator. Consider padding if more than 10 etc.
                    //We can pad if more than 10, but then we would need to get the number of matches fully, 
                    //and then reiterate through the loop. This would slow things down, but can be done easily.
                    EditorGUI.BeginDisabledGroup(replaceString == true);
                    useEnum = EditorGUILayout.Toggle(new GUIContent("Tally Objects", "Add a number at the end of each object in sequential order - Does not work with partial renaming"), useEnum);
                    EditorGUI.EndDisabledGroup();
                    EditorGUI.BeginDisabledGroup(useEnum == false);

                    //Decide whether or not to use a prefix, and obtain the starting number           
                    startEnum = EditorGUILayout.IntField(new GUIContent("Starting Number", "The first number in your sequence"), startEnum);
                    prefixInstead = EditorGUILayout.Toggle(new GUIContent("Prefix Instead", "Put the number in the beginning instead"), prefixInstead);
                    EditorGUI.EndDisabledGroup();
                    EditorGUILayout.Space();                       

                    //Perform a partial rename instead of a full rename
                    EditorGUI.BeginDisabledGroup(useEnum == true);
                    replaceString = EditorGUILayout.Toggle(new GUIContent("Partial Rename", "Replace only a portion of the name - Does not work with Tally Objects"), replaceString);
                    EditorGUI.EndDisabledGroup();
                    EditorGUI.BeginDisabledGroup(replaceString == false);
                        
                    //Partial rename search match
                    replaceThis = EditorGUILayout.TextField(new GUIContent("Rename only this part", "The text you wish to replace"), replaceThis);
                    partialCaseSentive = EditorGUILayout.Toggle(new GUIContent("Case sensitive", "Must the case match exactly"), partialCaseSentive);
                    EditorGUI.EndDisabledGroup();
                    EditorGUILayout.Space();

                    EditorGUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                        
                    //Perform the actual rename
                    if (GUILayout.Button("Rename", GUILayout.Width(160), GUILayout.Height(20)))
                        Rename();
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.Space();
                    EditorGUILayout.Space();
                    EditorGUILayout.EndVertical();
                }

                //Rigidbody Foldout
                rigidbodyFold = EditorGUILayout.Foldout(rigidbodyFold, new GUIContent("Rigidbody Tool", "Alter rigidbody settings"), myFoldoutStyle);
                if (rigidbodyFold)
                {
                    EditorGUILayout.BeginVertical(GUI.skin.box);
                    EditorGUILayout.Space();

                    //Perform actions on children objects
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.FlexibleSpace();
                        includeChildrenRigidbodies = EditorGUILayout.Toggle(new GUIContent("Include Children", "Also update child object rigidbodies (can be filtered via the search)"), includeChildrenRigidbodies);
                        GUILayout.FlexibleSpace();
                    }
                    GUILayout.EndHorizontal();

                    EditorGUILayout.Space();

                    //Change isKinematic settings
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button("Disable isKinematic", GUILayout.Width(160), GUILayout.Height(20)))
                        {
                            DisableIsKinematic();
                        }

                        GUILayout.FlexibleSpace();
                        
                        if (GUILayout.Button("Enable isKinematic", GUILayout.Width(160), GUILayout.Height(20)))
                        {
                            EnableIsKinematic();
                        }

                        GUILayout.FlexibleSpace();
                    }
                    GUILayout.EndHorizontal();

                    EditorGUILayout.Space();

                    //Change Use Gravity settings
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.FlexibleSpace();

                        if (GUILayout.Button("Disable Use Gravity", GUILayout.Width(160), GUILayout.Height(20)))
                        {
                            DisableUseGravity();
                        }
                        GUILayout.FlexibleSpace();

                        if (GUILayout.Button("Enable Use Gravity", GUILayout.Width(160), GUILayout.Height(20)))
                        {
                            EnableUseGravity();
                        }
                        GUILayout.FlexibleSpace();
                    }
                    GUILayout.EndHorizontal();
                    EditorGUILayout.Space();

                    //Add or remove rigidbodies
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.FlexibleSpace();

                        if (GUILayout.Button("Remove Rigidbodies", GUILayout.Width(160), GUILayout.Height(20)))
                        {
                            RemoveRigidBody();
                        }
                        GUILayout.FlexibleSpace();

                        if (GUILayout.Button("Add Rigidbodies", GUILayout.Width(160), GUILayout.Height(20)))
                        {
                            AddRigidBody();
                        }
                        GUILayout.FlexibleSpace();
                    }
                    GUILayout.EndHorizontal();
                    EditorGUILayout.Space();

                    EditorGUILayout.Space();
                    EditorGUILayout.EndVertical();
                }

                //Colliders Foldout
                collidersFold = EditorGUILayout.Foldout(collidersFold, new GUIContent("Colliders Tool", "Alter collider settings"), myFoldoutStyle);
                if (collidersFold)
                {
                    EditorGUILayout.BeginVertical(GUI.skin.box);
                    EditorGUILayout.Space();

                    //Perform actions on children objects
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.FlexibleSpace();
                        includeChildrenColliders = EditorGUILayout.Toggle(new GUIContent("Include Children", "Also update child object colliders (can be filtered via the search)"), includeChildrenColliders);
                        GUILayout.FlexibleSpace();
                    }
                    GUILayout.EndHorizontal();

                    EditorGUILayout.Space();

                    //Enable/Disable colliders
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button("Disable Colliders", GUILayout.Width(160), GUILayout.Height(20)))
                        {
                            DisableColliders();
                        }

                        GUILayout.FlexibleSpace();

                        if (GUILayout.Button("Enable Colliders", GUILayout.Width(160), GUILayout.Height(20)))
                        {
                            EnableColliders();
                        }

                        GUILayout.FlexibleSpace();
                    }
                    GUILayout.EndHorizontal();

                    EditorGUILayout.Space();

                    //Change isTrigger settings
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button("Disable isTrigger", GUILayout.Width(160), GUILayout.Height(20)))
                        {
                            DisableIsTrigger();
                        }

                        GUILayout.FlexibleSpace();

                        if (GUILayout.Button("Enable isTrigger", GUILayout.Width(160), GUILayout.Height(20)))
                        {
                            EnableIsTrigger();
                        }
                        GUILayout.FlexibleSpace();
                    }
                    GUILayout.EndHorizontal();
                    EditorGUILayout.Space();

                    //Set mesh collider to convex
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button("Disable Convex", GUILayout.Width(160), GUILayout.Height(20)))
                        {
                            DisableConvex();
                        }

                        GUILayout.FlexibleSpace();

                        if (GUILayout.Button("Enable Convex", GUILayout.Width(160), GUILayout.Height(20)))
                        {
                            EnableConvex();
                        }
                        GUILayout.FlexibleSpace();
                    }
                    GUILayout.EndHorizontal();
                    EditorGUILayout.Space();


                    //Add or Remove specific colliders -> Calls the ColliderWindow window.
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.FlexibleSpace();

                        if (GUILayout.Button("Remove Colliders", GUILayout.Width(160), GUILayout.Height(20)))
                        {                                
                            ColliderWindow aWindow = CreateInstance<ColliderWindow>();
                            aWindow.colliderMode = 1;
                            aWindow.titleContent.text = "Remove";
                            aWindow.position = new Rect(Screen.width / 2, Screen.height / 2, 400, 100);                                
                                
                            //ColliderMode sets whether to remove or add colliders
                            colliderMode = 1;
                            aWindow.Show();
                        }
                        GUILayout.FlexibleSpace();

                        if (GUILayout.Button("Add Colliders", GUILayout.Width(160), GUILayout.Height(20)))
                        {                               
                            ColliderWindow aWindow = CreateInstance<ColliderWindow>();
                            aWindow.colliderMode = 0;
                            aWindow.titleContent.text = "Add";
                            aWindow.position = new Rect(Screen.width / 2, Screen.height / 2, 400, 100);

                            //ColliderMode sets whether to remove or add colliders
                            colliderMode = 0;
                            aWindow.Show();
                        }
                        GUILayout.FlexibleSpace();
                    }
                    GUILayout.EndHorizontal();
                    EditorGUILayout.Space();

                    EditorGUILayout.Space();
                    EditorGUILayout.EndVertical();
                }

                //Renderers Foldout
                renderersFold = EditorGUILayout.Foldout(renderersFold, new GUIContent("Renderers Tool", "Alter renderer settings"), myFoldoutStyle);
                if (renderersFold)
                {
                    EditorGUILayout.BeginVertical(GUI.skin.box);
                    EditorGUILayout.Space();

                    //Perform actions on children objects
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.FlexibleSpace();
                        includeChildrenRenderers = EditorGUILayout.Toggle(new GUIContent("Include Children", "Also update child object renderers  (can be filtered via the search)"), includeChildrenRenderers);
                        GUILayout.FlexibleSpace();
                    }
                    GUILayout.EndHorizontal();

                    EditorGUILayout.Space();
                        
                    //Enable/Disable Renderers
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button("Disable Renderers", GUILayout.Width(160), GUILayout.Height(20)))
                        {
                            DisableRenderers();
                        }

                        GUILayout.FlexibleSpace();

                        if (GUILayout.Button("Enable Renderers", GUILayout.Width(160), GUILayout.Height(20)))
                        {
                            EnableRenderers();
                        }

                        GUILayout.FlexibleSpace();
                    }
                    GUILayout.EndHorizontal();

                    EditorGUILayout.Space();

                    //Change shadow settings for renderers
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.FlexibleSpace();

                        if (GUILayout.Button("Cast Shadows Off", GUILayout.Width(160), GUILayout.Height(20)))
                        {
                            DisableCastShadows();
                        }
                        GUILayout.FlexibleSpace();

                        if (GUILayout.Button("Cast Shadows On", GUILayout.Width(160), GUILayout.Height(20)))
                        {
                            EnableCastShadows();
                        }
                        GUILayout.FlexibleSpace();
                    }
                    GUILayout.EndHorizontal();
                    EditorGUILayout.Space();

                    //Change shadow settings for renderers
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button("Two-Sided Shadows", GUILayout.Width(160), GUILayout.Height(20)))
                        {
                            EnableCastShadowsTwoSided();
                        }
                        GUILayout.FlexibleSpace();

                        if (GUILayout.Button("Shadows Only", GUILayout.Width(160), GUILayout.Height(20)))
                        {
                            EnableCastShadowsOnly();
                        }
                        GUILayout.FlexibleSpace();
                    }
                    GUILayout.EndHorizontal();
                    EditorGUILayout.Space();
                    EditorGUILayout.Space();
                    EditorGUILayout.EndVertical();
                }

                //This is the Scripts Foldout. Originally it was planned as a component foldout, but changed to scripts only.
                //If the desire is great enough in future for specific component types, we can add a component tool also.
                scriptsFold = EditorGUILayout.Foldout(scriptsFold, new GUIContent("Scripts Tool", "Add scripts to GameObjects"), myFoldoutStyle);
                if (scriptsFold)
                {
                    EditorGUILayout.BeginVertical(GUI.skin.box);
                    EditorGUILayout.Space();

                    //Perform actions on children objects
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.FlexibleSpace();
                        includeChildrenScripts = EditorGUILayout.Toggle(new GUIContent("Include Children", "Also add scripts to child objects (can be filtered via the search)"), includeChildrenScripts);
                        GUILayout.FlexibleSpace();
                    }
                    GUILayout.EndHorizontal();

                    EditorGUILayout.Space();

                    //Add scripts area. Cannot add if we are removing.
                    EditorGUI.BeginDisabledGroup(removeScript == true);
                    addScript = EditorGUILayout.Toggle(new GUIContent("Add Script", "Add a specific script"), addScript);
                    EditorGUI.BeginDisabledGroup(addScript == false);
                    //we now show the search input field            
                    scriptToAdd = (MonoScript)EditorGUILayout.ObjectField(new GUIContent("Add Script", "Select a script to attach"), scriptToAdd, typeof(MonoScript), false);
                    EditorGUI.EndDisabledGroup();
                    EditorGUI.EndDisabledGroup();

                    //Remove scripts are. Cannot remove if we are adding.
                    EditorGUI.BeginDisabledGroup(addScript == true);
                    removeScript = EditorGUILayout.Toggle(new GUIContent("Remove Script", "Remove a specific script if it is attached"), removeScript);
                    EditorGUI.BeginDisabledGroup(removeScript == false);
                    //we now show the search input field            
                    scriptToRemove = (MonoScript)EditorGUILayout.ObjectField(new GUIContent("Remove Script", "Select a script to remove"), scriptToRemove, typeof(MonoScript), false);
                    EditorGUI.EndDisabledGroup();
                    EditorGUI.EndDisabledGroup();

                    EditorGUILayout.Space();

                    EditorGUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();

                    if (GUILayout.Button("Remove Missing Scripts", GUILayout.Width(160), GUILayout.Height(20)))
                    {
                        RemoveEmptyScripts.RemoveMissingScriptReferences();
                    }
                    GUILayout.FlexibleSpace();

                    //The process button. Can only add or remove script per button press. Cannot combine add and remove in one action.
                    EditorGUI.BeginDisabledGroup((scriptToAdd == null && scriptToRemove == null) || (addScript == false && removeScript == false) || (addScript == true && scriptToAdd == null) || (removeScript == true && scriptToRemove == null));
                    if (GUILayout.Button("Process", GUILayout.Width(160), GUILayout.Height(20)))
                        AddRemoveScripts();

                    //RemoveMissingScriptReferences
                    EditorGUI.EndDisabledGroup();
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.Space();
                    EditorGUILayout.Space();
                    EditorGUILayout.EndVertical();
                }


                componentFold = EditorGUILayout.Foldout(componentFold, new GUIContent("Component Tool", "Add/Remove/Copy components or their values to GameObjects"), myFoldoutStyle);
                if (componentFold)
                {
                    Rect clickArea = EditorGUILayout.BeginVertical(GUI.skin.box);
                    EditorGUILayout.Space();

                    //Perform actions on children objects
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.FlexibleSpace();
                        includeChildrenComponents = EditorGUILayout.Toggle(new GUIContent("Include Children", "Also add/remove/copy components to child objects (can be filtered via the search)"), includeChildrenComponents);
                        GUILayout.FlexibleSpace();
                    }
                    GUILayout.EndHorizontal();

                    EditorGUILayout.Space();

                    //Perform actions on children objects
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.FlexibleSpace();

                        EditorGUI.BeginDisabledGroup(removeComp == true);
                        updateComp = EditorGUILayout.Toggle(new GUIContent("Update Component", "Update only existing component values"), updateComp);
                        EditorGUI.EndDisabledGroup();

                        GUILayout.FlexibleSpace();
                    }
                    GUILayout.EndHorizontal();

                    EditorGUILayout.Space();

                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.FlexibleSpace();

                        EditorGUI.BeginDisabledGroup(removeComp == true);
                        addComp = EditorGUILayout.Toggle(new GUIContent("Add Component", "Add the component if they do not exist, and set their values"), addComp);
                        EditorGUI.EndDisabledGroup();

                        GUILayout.FlexibleSpace();
                    }
                    GUILayout.EndHorizontal();

                    EditorGUILayout.Space();

                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.FlexibleSpace();

                        EditorGUI.BeginDisabledGroup(updateComp == true || addComp == true);
                        removeComp = EditorGUILayout.Toggle(new GUIContent("Remove Component", "Remove the components if they exist"), removeComp);
                        EditorGUI.EndDisabledGroup();

                        GUILayout.FlexibleSpace();
                    }
                    GUILayout.EndHorizontal();

                    EditorGUILayout.Space();

                    GUILayout.BeginVertical(GUI.skin.box);
                    {
                        //in order for a component to be displayed, it would need to be attached to a gameobject.
                        //we create an pseudo game object, paste the component on it, and display it here.
                        //newGameobject

                        //draw an area where newgameobject's components will be displayed.

                        //Rect anotherClickArea = EditorGUILayout.GetControlRect();
                        ////random usage to stop the console from moaning. I need to have this area defined above.
                        //anotherClickArea.width = anotherClickArea.width + 0.1f;

                        Event current = Event.current;

                        if (clickArea.Contains(current.mousePosition) && current.type == EventType.ContextClick)
                        {
                            //Do a thing, in this case a drop down menu
                            GenericMenu menu = new GenericMenu();

                            menu.AddItem(new GUIContent("Paste Component"), false, PasteComponentCallBack);
                            menu.ShowAsContext(); 
                        
                            current.Use();
                        }

                        bool msgDrawn = false;

                        if (compAdded)
                        {
                            try
                            {                                     
                                //EditorGUILayout.BeginVertical(GUI.skin.box);
                                foreach (Component aComponent in newGameobject.GetComponents(typeof(Component)))
                                {
                                    if (!(aComponent.GetType() == typeof(Transform)))
                                    {
                                        try
                                        {
                                            if (aComponent)
                                            {
                                                EditorGUILayout.LabelField("Component : " + aComponent.GetType().ToString().Replace("UnityEngine.",""), GUILayout.ExpandWidth(true), GUILayout.MinWidth(100f));

                                                SerializedObject serializedObject = new SerializedObject(aComponent);
                                                SerializedProperty firstProp = serializedObject.GetIterator();

                                                if (firstProp.hasVisibleChildren)
                                                {
                                                    GUILayout.BeginVertical(GUI.skin.box);
                                                    {                                                        
                                                        nonTransformCompFound = true;
                                                        if (aComponent)
                                                        {                                                             
                                                            Editor myEditor = Editor.CreateEditor(aComponent);
                                                            myEditor.DrawDefaultInspector();
                                                            bool editorAlreadyAdded = false;

                                                            if(myEditors.Count > 0)
                                                            {
                                                                foreach(Editor anEditor in myEditors)
                                                                {
                                                                    if(anEditor.GetType() == myEditor.GetType())
                                                                    {
                                                                        editorAlreadyAdded = true;
                                                                    }
                                                                }
                                                            }
                                                            else
                                                            {
                                                                editorAlreadyAdded = false;
                                                            }
                                                            
                                                            if(editorAlreadyAdded == false)
                                                            {
                                                                //only need to add this editor if it does not exist
                                                                myEditors.Add(myEditor);
                                                            }
                                                        }
                                                        EditorGUILayout.Space();
                                                    }
                                                    GUILayout.EndVertical();
                                                }
                                                else
                                                {
                                                    GUILayout.BeginVertical(GUI.skin.box);
                                                    {
                                                        nonTransformCompFound = true;                                                        
                                                        EditorGUILayout.LabelField("Component added, but nothing to display", GUILayout.ExpandWidth(true), GUILayout.MinWidth(100f));
                                                        EditorGUILayout.LabelField("No serializable values detected for '" + aComponent.GetType().ToString().Replace("UnityEngine.", "") + "'", GUILayout.ExpandWidth(true), GUILayout.MinWidth(100f));
                                                        EditorGUILayout.Space();
                                                    }
                                                    GUILayout.EndVertical();
                                                }
                                            }
                                        }
                                        catch
                                        {
                                            Debug.LogError("Unable to determine serializable properties");
                                        }
                                    }

                                }
                            }
                            catch
                            {
                                //object no longer exists.       
                                Debug.LogError("Cannot draw component window.");
                            }
                        }
                        else
                        {
                            msgDrawn = true;
                            GUILayout.FlexibleSpace();
                            GUILayout.BeginHorizontal();
                            {
                                GUILayout.FlexibleSpace();
                                EditorGUILayout.LabelField("Paste Component", GUILayout.ExpandWidth(false), GUILayout.Width(150f));
                                GUILayout.FlexibleSpace();
                            }
                            GUILayout.EndHorizontal();
                            GUILayout.FlexibleSpace();
                            EditorGUILayout.Space();
                            EditorGUILayout.Space();
                        }

                        if (nonTransformCompFound == false && msgDrawn == false)
                        {
                            GUILayout.FlexibleSpace();
                            GUILayout.BeginHorizontal();
                            {
                                GUILayout.FlexibleSpace();
                                EditorGUILayout.LabelField("Paste Component", GUILayout.ExpandWidth(false), GUILayout.Width(150f));
                                GUILayout.FlexibleSpace();
                            }
                            GUILayout.EndHorizontal();
                            GUILayout.FlexibleSpace();
                            EditorGUILayout.Space();
                            EditorGUILayout.Space();
                        }


                    }
                    GUILayout.EndVertical();

                    EditorGUILayout.Space();

                    EditorGUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();

                    //The process button. Can only add or remove script per button press. Cannot combine add and remove in one action.                    
                    if (GUILayout.Button("Clear", GUILayout.Width(160), GUILayout.Height(20)))
                    {
                        ClearComponents();
                        RemoveHelpers();
                    }
                    GUILayout.FlexibleSpace();

                    EditorGUI.BeginDisabledGroup((!CanProcessComponents()) || (compAdded == false));
                    if (GUILayout.Button("Process", GUILayout.Width(160), GUILayout.Height(20)))
                    {
                        ProcessComponents();
                    }
                    EditorGUI.EndDisabledGroup();

                    GUILayout.FlexibleSpace();
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.Space();
                    EditorGUILayout.Space();
                    EditorGUILayout.EndVertical();
                }

                EditorGUILayout.Space();
                EditorGUILayout.Space();

                //This is the event returned to this window from the ColliderWindow window.
                //If the commandname is listed below, it will perform an action based on the collidermode.
                Event e = Event.current;
                string commandName = e.commandName;                    
                if(commandName == "Sphere" || commandName == "Box" || commandName == "Capsule" || commandName == "Mesh")                    
                { 
                    e.commandName = string.Empty;
                    if (colliderMode == 0)
                    {
                        AddColliders(commandName);
                    }
                    else
                    {
                        RemoveColliders(commandName);
                    }                        
                }

                //This will close the window
                EditorGUILayout.BeginHorizontal();
                {
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("Close", GUILayout.Width(160), GUILayout.Height(20)))
                    {
                        ClearComponents();
                        RemoveHelpers();
                        RemoveFocus();
                        Close(); 
                    }                        
                    GUILayout.FlexibleSpace();
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.Space();

                //The message that this tool sends to the console.
                EditorGUILayout.LabelField(consoleOutput, GUILayout.ExpandWidth(true), GUILayout.Width(250f));

                EditorGUILayout.EndScrollView();
                EditorGUILayout.EndVertical();
            }
            catch (System.Exception ex)
            {
                Debug.LogError(ex.Message);
                Debug.LogError("If this error persists, please contact the developer for support.");
            }
        }


        #endregion

        #region ComponentControl

        /// <summary>
        /// These are the functions that perform the component copying tool actions.
        /// </summary>       

        private void CreateEmptyGameObject()
        {
            try
            {
                if (newGameobject == null)
                {
                    bool helperExists = false;
                    GameObject[] theObjects = FindObjectsOfType<GameObject>();
                    foreach (GameObject go in theObjects)
                    {
                        if (go.name == "MultiEditTools Helper")
                        {
                            helperExists = true;
                            newGameobject = go;
                            createdObjects.Add(go);
                            EditorApplication.RepaintHierarchyWindow();
                        }
                    }

                    if (!helperExists)
                    {
                        newGameobject = new GameObject();
                        newGameobject.name = "MultiEditTools Helper";
                        createdObjects.Add(newGameobject);
                        EditorApplication.RepaintHierarchyWindow();
                    }
                }
            }
            catch
            {
                Debug.LogError("Unable to create MultiEditTools Helper GameObject");
            }
        }

        private void ClearComponents()
        {
            RemoveFocus();

            try
            {
                foreach (GameObject go in createdObjects)
                {
                    try
                    {
                        if (go != null)
                        {
                            int componentCount = 0;
                            foreach (Component aComponent in newGameobject.GetComponents(typeof(Component)))
                            {
                                if (!(aComponent.GetType() == typeof(Transform)))
                                {
                                    componentCount++;
                                }
                            }

                            int counter = 0;
                            //Let's say we run this only for a maximum of (component count * 3) times 
                            int counterLimit = componentCount * 3;

                            while (componentCount > 0 && counter < counterLimit)
                            {
                                foreach (Component aComponent in newGameobject.GetComponents(typeof(Component)))
                                {
                                    if (!(aComponent.GetType() == typeof(Transform)))
                                    {
                                        compAdded = false;
                                        if (newGameobject.CanDestroy(aComponent.GetType()))
                                        {
                                            Undo.DestroyObjectImmediate(aComponent);
                                            componentCount--;
                                        }
                                    }
                                }
                                counter++;
                            }
                        }
                    }
                    catch
                    {
                        //The game object might not exist.
                    }
                }
                EditorApplication.RepaintHierarchyWindow();
            }
            catch
            {
                Debug.LogError("Unable to clear the helper components");
            }
            
                    
        }

        private void RemoveFocus()
        {
            Selection.activeGameObject = null;
            EditorApplication.RepaintHierarchyWindow();
            SceneView.RepaintAll();
            if(myEditors.Count > 0)
            {
                foreach (Editor myEditor in myEditors)
                {
                    DestroyImmediate(myEditor);
                }
                myEditors.Clear();
            }
        }

        private void RemoveHelpers()
        {
            RemoveFocus();

            try
            {
                GameObject[] theObjects = FindObjectsOfType<GameObject>();
                foreach (GameObject go in theObjects)
                {
                    try
                    {
                        if (go.name == "MultiEditTools Helper")
                        {
                            compAdded = false;                            
                            Undo.DestroyObjectImmediate(go);                              
                        }
                    }
                    catch
                    {
                        //The game object might not exist.
                    } 
                    finally
                    {
                        if (newGameobject != null)
                        {
                            compAdded = false;
                            newGameobject = null;
                            Undo.DestroyObjectImmediate(newGameobject);                            
                        }
                        createdObjects.Clear();
                    }
                } 
                EditorApplication.RepaintHierarchyWindow();                
            }
            catch
            {
                Debug.LogError("Unable to destroy the helper GameObjects");
            }
        }

        private void ProcessComponents()
        {

            if (addComp == true || updateComp == true)
            {
                CopyComponents();
            }

            if (removeComp == true)
            {
                RemoveComponents();
            }
        }

        private bool CanProcessComponents()
        {
            //we cannot have remove true and any of the others true
            //we cannot have all true or all false            

            if (removeComp == false && addComp == false && updateComp == false)
                return false;

            if (removeComp == true && addComp == true && updateComp == true)
                return false;

            if (removeComp == true && addComp == false && updateComp == true)
                return false;

            if (removeComp == true && addComp == true && updateComp == false)
                return false;

            return true;
        }

        private void CopyComponents()
        {
            GameObject[] objs = Selection.gameObjects;
            //Here we have the gameobjects selected.
            int numberOfTransforms = objs.Length;
            bool showProgressBar = numberOfTransforms >= SelectionThresholdForProgressBar;
            int changedObjectsCount = 0;
            string keywordAction = "Copying";
            if (addComp) keywordAction = "Adding/Copying";
            string keywordComplete = "copied";
            if (addComp) keywordComplete = "added/copied";

            try
            {
                for (int i = 0; i < numberOfTransforms; i++)
                {

                    if (showProgressBar)
                    {
                        EditorUtility.DisplayProgressBar(keywordAction + " Component", keywordAction + " component (" + i + "/" + numberOfTransforms + " parent objects)",
                            (float)i / (float)numberOfTransforms);
                    }

                    Transform[] allTransforms;

                    //Are we possibly adding scripts to child objects?
                    if (includeChildrenComponents)
                    {
                        allTransforms = objs[i].GetComponentsInChildren<Transform>();
                    }
                    else
                    {
                        allTransforms = objs[i].GetComponents<Transform>();
                    }

                    foreach (Transform childTransform in allTransforms)
                    {
                        if (useSearch)
                        {
                           
                            bool matchedSearch = false;

                            if(sType == SEARCHTYPE.NAME)
                            {
                                if (searchCaseSentive)
                                {
                                    switch (op)
                                    {
                                        case SEARCHOPTIONS.STARTS_WITH:
                                            if (childTransform.name.StartsWith(searchString))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;

                                        case SEARCHOPTIONS.CONTAINS:
                                            if (childTransform.name.Contains(searchString))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;

                                        case SEARCHOPTIONS.ENDS_WITH:
                                            if (childTransform.name.EndsWith(searchString))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;
                                    }
                                }
                                else
                                {
                                    switch (op)
                                    {
                                        case SEARCHOPTIONS.STARTS_WITH:
                                            if (childTransform.name.ToUpper().StartsWith(searchString.ToUpper()))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;

                                        case SEARCHOPTIONS.CONTAINS:
                                            if (childTransform.name.ToUpper().Contains(searchString.ToUpper()))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;

                                        case SEARCHOPTIONS.ENDS_WITH:
                                            if (childTransform.name.ToUpper().EndsWith(searchString.ToUpper()))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;
                                    }
                                }
                            }

                            if(sType == SEARCHTYPE.TAG)
                            {
                                if (searchTagCaseSentive)
                                {
                                    if (childTransform.tag.ToUpper().Equals(searchTag.ToUpper()))
                                    {
                                        matchedSearch = true;
                                    }
                                }
                                else
                                {
                                    if(childTransform.CompareTag(searchTag))
                                    {
                                        matchedSearch = true;
                                    }
                                }
                            }

                            if(sType == SEARCHTYPE.LAYER)
                            {
                                if(childTransform.gameObject.layer == searchLayer)
                                {
                                    matchedSearch = true;
                                }
                            }
                            

                            //We need to only use go as the game object if it either matches the seach, or else do everything
                            if (matchedSearch == true)
                            {
                                try
                                {
                                    //we need to copy the component from our helper object.
                                    foreach (Component aComponent in newGameobject.GetComponents(typeof(Component)))
                                    {
                                        if (!(aComponent.GetType() == typeof(Transform)))
                                        {
                                            //If they're the non-transform component, let's add them.
                                            //we then check if a similar component exists on the object already.
                                            //if so, paste the values. 
                                            //if not, and add is checked, copy the component.
                                            var componentToAdd = childTransform.gameObject.GetComponent(aComponent.GetType());

                                            if (UnityEditorInternal.ComponentUtility.CopyComponent(aComponent))
                                            {
                                                if (componentToAdd == null)
                                                {
                                                    if (addComp)
                                                    {
                                                        //It does not exist
                                                        try
                                                        {
                                                            //Record Undo statement. Add the script.
                                                            Undo.AddComponent(childTransform.gameObject, aComponent.GetType());
                                                            if (updateComp)
                                                            {
                                                                //record Undo statement. Paste component Values
                                                                Undo.RegisterFullObjectHierarchyUndo(childTransform.gameObject, "Update Component Values");
                                                                UnityEditorInternal.ComponentUtility.PasteComponentValues(childTransform.gameObject.GetComponent(aComponent.GetType()));
                                                            }
                                                            changedObjectsCount++;

                                                        }
                                                        catch
                                                        {
                                                            //Sometimes certain script types are incompatible.
                                                            Debug.LogError("Unable to add/update component to object named : '" + childTransform.name + "'");
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    //It exists
                                                    try
                                                    {
                                                        if (updateComp)
                                                        {
                                                            //record Undo statement. Paste component Values
                                                            Undo.RegisterFullObjectHierarchyUndo(childTransform.gameObject, "Update Component Values");
                                                            UnityEditorInternal.ComponentUtility.PasteComponentValues(componentToAdd);
                                                            changedObjectsCount++;
                                                        }
                                                    }
                                                    catch
                                                    {
                                                        //Sometimes certain script types are incompatible.
                                                        Debug.LogError("Unable to update component to object named : '" + childTransform.name + "'");
                                                    }
                                                }
                                            }

                                        }
                                    }

                                }
                                catch
                                {
                                    Debug.LogError("Unable to add or update component to object named : '" + childTransform.name + "'");
                                }
                            }
                        }
                        else
                        {
                            //No search filtering. Add component to transform.
                            try
                            {
                                //we need to copy the component from our helper object.
                                foreach (Component aComponent in newGameobject.GetComponents(typeof(Component)))
                                {
                                    if (!(aComponent.GetType() == typeof(Transform)))
                                    {
                                        //If they're the non-transform component, let's add them.
                                        //we then check if a similar component exists on the object already.
                                        //if so, paste the values. 
                                        //if not, and add is checked, copy the component.
                                        var componentToAdd = childTransform.gameObject.GetComponent(aComponent.GetType());

                                        if (UnityEditorInternal.ComponentUtility.CopyComponent(aComponent))
                                        {
                                            if (componentToAdd == null)
                                            {
                                                if (addComp)
                                                {
                                                    //It does not exist
                                                    try
                                                    {
                                                        //Record Undo statement. Add the script.
                                                        Undo.AddComponent(childTransform.gameObject, aComponent.GetType());
                                                        //record Undo statement. Paste component Values
                                                        if (updateComp)
                                                        {
                                                            Undo.RegisterFullObjectHierarchyUndo(childTransform.gameObject, "Update Component Values");
                                                            UnityEditorInternal.ComponentUtility.PasteComponentValues(childTransform.gameObject.GetComponent(aComponent.GetType()));
                                                            changedObjectsCount++;
                                                        }
                                                    }
                                                    catch
                                                    {
                                                        //Sometimes certain script types are incompatible.
                                                        Debug.LogError("Unable to add/update component to object named : '" + childTransform.name + "'");
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                //It exists
                                                try
                                                {
                                                    if (updateComp)
                                                    {
                                                        //record Undo statement. Paste component Values
                                                        Undo.RegisterFullObjectHierarchyUndo(childTransform.gameObject, "Update Component Values");
                                                        UnityEditorInternal.ComponentUtility.PasteComponentValues(componentToAdd);
                                                        changedObjectsCount++;
                                                    }
                                                }
                                                catch
                                                {
                                                    //Sometimes certain script types are incompatible.
                                                    Debug.LogError("Unable to update component to object named : '" + childTransform.name + "'");
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            catch
                            {
                                Debug.LogError("Unable to add or update component to object named : '" + childTransform.name + "'");
                            }
                        }
                    }
                }
            }
            finally
            {
                //Show Results
                if (showProgressBar)
                {
                    EditorUtility.ClearProgressBar();
                }

                if (changedObjectsCount > 0)
                {
                    if (changedObjectsCount == 1)
                    {
                        Debug.LogFormat("Component " + keywordComplete + " on 1 Gameobject");
                        consoleOutput = "Component " + keywordComplete + " on 1 Gameobject";
                    }
                    else
                    {
                        Debug.LogFormat("Component " + keywordComplete + " on {0} Gameobjects", changedObjectsCount);
                        consoleOutput = string.Format("Component " + keywordComplete + " on {0} Gameobjects", changedObjectsCount);
                    }
                }
                else
                {
                    Debug.LogFormat("No components " + keywordComplete);
                    consoleOutput = "No components " + keywordComplete;
                }

            }

        }

        private void RemoveComponents()
        {
            GameObject[] objs = Selection.gameObjects;
            //Here we have the gameobjects selected.
            int numberOfTransforms = objs.Length;
            bool showProgressBar = numberOfTransforms >= SelectionThresholdForProgressBar;
            int changedObjectsCount = 0;

            try
            {
                for (int i = 0; i < numberOfTransforms; i++)
                {

                    if (showProgressBar)
                    {
                        EditorUtility.DisplayProgressBar("Remove Component", "Removing component (" + i + "/" + numberOfTransforms + " parent objects)",
                            (float)i / (float)numberOfTransforms);
                    }

                    Transform[] allTransforms;

                    //Must we possibly remove components from child objects also?
                    if (includeChildrenComponents)
                    {
                        allTransforms = objs[i].GetComponentsInChildren<Transform>();
                    }
                    else
                    {
                        allTransforms = objs[i].GetComponents<Transform>();
                    }


                    foreach (Transform childTransform in allTransforms)
                    {
                        if (useSearch)
                        {
                            bool matchedSearch = false;

                            if (sType == SEARCHTYPE.NAME)
                            {
                                if (searchCaseSentive)
                                {
                                    switch (op)
                                    {
                                        case SEARCHOPTIONS.STARTS_WITH:
                                            if (childTransform.name.StartsWith(searchString))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;

                                        case SEARCHOPTIONS.CONTAINS:
                                            if (childTransform.name.Contains(searchString))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;

                                        case SEARCHOPTIONS.ENDS_WITH:
                                            if (childTransform.name.EndsWith(searchString))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;
                                    }
                                }
                                else
                                {
                                    switch (op)
                                    {
                                        case SEARCHOPTIONS.STARTS_WITH:
                                            if (childTransform.name.ToUpper().StartsWith(searchString.ToUpper()))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;

                                        case SEARCHOPTIONS.CONTAINS:
                                            if (childTransform.name.ToUpper().Contains(searchString.ToUpper()))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;

                                        case SEARCHOPTIONS.ENDS_WITH:
                                            if (childTransform.name.ToUpper().EndsWith(searchString.ToUpper()))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;
                                    }
                                }
                            }

                            if (sType == SEARCHTYPE.TAG)
                            {
                                if (searchTagCaseSentive)
                                {
                                    if (childTransform.tag.ToUpper().Equals(searchTag.ToUpper()))
                                    {
                                        matchedSearch = true;
                                    }
                                }
                                else
                                {
                                    if (childTransform.CompareTag(searchTag))
                                    {
                                        matchedSearch = true;
                                    }
                                }
                            }

                            if (sType == SEARCHTYPE.LAYER)
                            {
                                if (childTransform.gameObject.layer == searchLayer)
                                {
                                    matchedSearch = true;
                                }
                            }

                            //We need to only use go as the game object if it either matches the seach, or else do everything
                            if (matchedSearch == true)
                            {
                                try
                                {
                                    foreach (Component aComponent in newGameobject.GetComponents(typeof(Component)))
                                    {
                                        if (!(aComponent.GetType() == typeof(Transform)))
                                        {
                                            //If they're the non-transform component, let's add them.
                                            //we then check if a similar component exists on the object already.
                                            //if so, paste the values. 
                                            //if not, and add is checked, copy the component.                                           
                                            try
                                            {
                                                Component[] componentsToRemove = childTransform.gameObject.GetComponents(aComponent.GetType());

                                                foreach (Component aComp in componentsToRemove)
                                                {
                                                    if (aComp != null)
                                                    {
                                                        //Record Undo statements. Remove script.
                                                        if (childTransform.gameObject.CanDestroy(aComp.GetType()))
                                                        {
                                                            Undo.DestroyObjectImmediate(aComp);
                                                            changedObjectsCount++;
                                                        }
                                                        else
                                                        {
                                                            Debug.Log("Unable to remove component on object named : " + childTransform.name + " due to dependencies");
                                                        }
                                                    }
                                                }
                                            }
                                            catch
                                            {
                                                Debug.LogError("Unable to remove component on object named : '" + childTransform.name + "'");
                                            }

                                        }
                                    }
                                }
                                catch
                                {
                                    Debug.LogError("Unable to remove component on object named : '" + childTransform.name + "'");
                                }
                            }
                        }
                        else
                        {
                            //No search filtering. 
                            try
                            {
                                try
                                {
                                    foreach (Component aComponent in newGameobject.GetComponents(typeof(Component)))
                                    {
                                        if (!(aComponent.GetType() == typeof(Transform)))
                                        {
                                            //If they're the non-transform component, let's add them.
                                            //we then check if a similar component exists on the object already.
                                            //if so, paste the values. 
                                            //if not, and add is checked, copy the component.                                           
                                            try
                                            {


                                                Component[] componentsToRemove = childTransform.gameObject.GetComponents(aComponent.GetType());

                                                foreach (Component aComp in componentsToRemove)
                                                {
                                                    if (aComp != null)
                                                    {
                                                        if (childTransform.gameObject.CanDestroy(aComp.GetType()))
                                                        {
                                                            //Record Undo statements. Remove script.
                                                            Undo.DestroyObjectImmediate(aComp);
                                                            changedObjectsCount++;
                                                        }
                                                        else
                                                        {
                                                            Debug.Log("Unable to remove component on object named : " + childTransform.name + " due to dependencies");
                                                        }
                                                    }
                                                }
                                            }
                                            catch
                                            {
                                                Debug.LogError("Unable to remove component on object named : '" + childTransform.name + "'");
                                            }
                                        }
                                    }
                                }
                                catch
                                {
                                    Debug.LogError("Unable to remove component on object named : '" + childTransform.name + "'");
                                }
                            }
                            catch
                            {
                                Debug.LogError("Unable to remove component on object named : '" + childTransform.name + "'");
                            }
                        }
                    }
                }
            }
            finally
            {
                //Output results
                if (showProgressBar)
                {
                    EditorUtility.ClearProgressBar();
                }

                if (changedObjectsCount > 0)
                {
                    if (changedObjectsCount == 1)
                    {
                        Debug.LogFormat("1 component removed");
                        consoleOutput = "1 component removed";
                    }
                    else
                    {
                        Debug.LogFormat("{0} components removed", changedObjectsCount);
                        consoleOutput = string.Format("{0} components removed", changedObjectsCount);
                    }
                }
                else
                {
                    Debug.LogFormat("No components removed");
                    consoleOutput = "No components removed";
                }

            }
        }

        void PasteComponentCallBack()
        {
            try
            {
                try
                {
                    ClearComponents();
                    CreateEmptyGameObject();
                }
                catch
                {
                    Debug.LogError("Unable to control helper GameObjects");
                }

                if (newGameobject)
                {
                    if (UnityEditorInternal.ComponentUtility.PasteComponentAsNew(newGameobject))
                    {
                        compAdded = true;
                        EditorApplication.RepaintHierarchyWindow();
                    }
                    else
                    {
                        Debug.Log("Please copy a valid component to add");
                        RemoveHelpers();
                    }
                }
            }
            catch
            {
                Debug.LogError("Unable to run paste component callback");
            }
        }
        #endregion

        #region RenameControl

        /// <summary>
        /// These are the functions that perform the rename tool actions.
        /// </summary>

        static void Rename()
        {                
            GameObject[] objs = Selection.gameObjects;
            //Here we have the gameobjects selected.
            int numberOfTransforms = objs.Length;
            bool showProgressBar = numberOfTransforms >= SelectionThresholdForProgressBar;
            int changedObjectsCount = 0;
            int enumStart = startEnum;

            try
            {
                for (int i = 0; i < numberOfTransforms; i++)
                {

                    if (showProgressBar)
                    {
                        EditorUtility.DisplayProgressBar("Rename GameObjects", "Checking GameObjects For Rename (" + i + "/" + numberOfTransforms + " parent objects)",
                            (float)i / (float)numberOfTransforms);
                    }

                    try
                    {

                        //Ok so the conditions are:

                        //We either use a search or not. If we do, we check for all game objects that match.
                        //Else everything matches.

                        //either way, the rename can be full, partial, or full with tally.

                        Transform[] allTransforms;

                        //If we include all children in the rename, then we have to take the children of the selected objects into our transform list.
                        if (includeChildrenRename)
                        {
                            allTransforms = objs[i].GetComponentsInChildren<Transform>();
                        }
                        else
                        {
                            allTransforms = objs[i].GetComponents<Transform>();
                        }

                        foreach (Transform childTransform in allTransforms)
                        {
                            //we cannot tally with a partial rename. We only tally with a full rename
                            bool matchedSearch = false;

                            if (useSearch)
                            {                                

                                if (sType == SEARCHTYPE.NAME)
                                {
                                    if (searchCaseSentive)
                                    {
                                        switch (op)
                                        {
                                            case SEARCHOPTIONS.STARTS_WITH:
                                                if (childTransform.name.StartsWith(searchString))
                                                {
                                                    matchedSearch = true;
                                                }
                                                break;

                                            case SEARCHOPTIONS.CONTAINS:
                                                if (childTransform.name.Contains(searchString))
                                                {
                                                    matchedSearch = true;
                                                }
                                                break;

                                            case SEARCHOPTIONS.ENDS_WITH:
                                                if (childTransform.name.EndsWith(searchString))
                                                {
                                                    matchedSearch = true;
                                                }
                                                break;
                                        }
                                    }
                                    else
                                    {
                                        switch (op)
                                        {
                                            case SEARCHOPTIONS.STARTS_WITH:
                                                if (childTransform.name.ToUpper().StartsWith(searchString.ToUpper()))
                                                {
                                                    matchedSearch = true;
                                                }
                                                break;

                                            case SEARCHOPTIONS.CONTAINS:
                                                if (childTransform.name.ToUpper().Contains(searchString.ToUpper()))
                                                {
                                                    matchedSearch = true;
                                                }
                                                break;

                                            case SEARCHOPTIONS.ENDS_WITH:
                                                if (childTransform.name.ToUpper().EndsWith(searchString.ToUpper()))
                                                {
                                                    matchedSearch = true;
                                                }
                                                break;
                                        }
                                    }
                                }

                                if (sType == SEARCHTYPE.TAG)
                                {
                                    if (searchTagCaseSentive)
                                    {
                                        if (childTransform.tag.ToUpper().Equals(searchTag.ToUpper()))
                                        {
                                            matchedSearch = true;
                                        }
                                    }
                                    else
                                    {
                                        if (childTransform.CompareTag(searchTag))
                                        {
                                            matchedSearch = true;
                                        }
                                    }
                                }

                                if (sType == SEARCHTYPE.LAYER)
                                {
                                    if (childTransform.gameObject.layer == searchLayer)
                                    {
                                        matchedSearch = true;
                                    }
                                }
                            }
                            else
                            {
                                //if we don't use search, every object in the collection will be renamed (every object matches)
                                matchedSearch = true;
                            }

                            //If we match a search or no search is used (we always match if no search is used)
                            if (matchedSearch)
                            {
                                if (replaceString)
                                {
                                    //partial rename
                                    if (partialCaseSentive)
                                    {
                                        //So we search for the part to replace and replace it.
                                        //we search anywhere in the string and replace.
                                        //The Undo command registers an undo event for this action
                                        Undo.RegisterFullObjectHierarchyUndo(childTransform, "Rename GameObject(s)");
                                        childTransform.name = childTransform.name.Replace(replaceThis, withThis);
                                        changedObjectsCount++;                                        
                                    }
                                    else
                                    {
                                        //we have partial search insensitivity
                                        int matchedIndex = childTransform.name.ToUpper().IndexOf(replaceThis.ToUpper());
                                        if (matchedIndex >= 0)
                                        {
                                            //we find the upper match, rename the part we're changing with the new part only, and leave the rest of the string as is.                                                            
                                            Undo.RegisterFullObjectHierarchyUndo(childTransform, "Rename GameObject(s)");
                                            childTransform.name = childTransform.name.Substring(0, matchedIndex) + withThis + childTransform.name.Substring(matchedIndex + replaceThis.Length);
                                            changedObjectsCount++;
                                        }
                                    }
                                }
                                else
                                {
                                    //we rename the whole name and use an enum
                                    if (useEnum)
                                    {
                                        //add an enum at the beginning instead   
                                        if (prefixInstead)
                                        {
                                            Undo.RegisterFullObjectHierarchyUndo(childTransform, "Rename GameObject(s)");
                                            childTransform.name = enumStart.ToString() + withThis;
                                            changedObjectsCount++;
                                            enumStart++;
                                        }
                                        else
                                        {
                                            //add an enum at the end. 
                                            //For future versions, consider padding based on string length of number of items e.g. 10 items will be left padded with zeros to length 2, 100 will be left padded with zeros to length 3.
                                            Undo.RegisterFullObjectHierarchyUndo(childTransform, "Rename GameObject(s)");
                                            childTransform.name = withThis + enumStart.ToString();
                                            changedObjectsCount++;
                                            enumStart++;
                                        }
                                    }
                                    else
                                    {
                                        //rename without an enum
                                        Undo.RegisterFullObjectHierarchyUndo(childTransform, "Rename GameObject(s)");
                                        childTransform.name = withThis;
                                        changedObjectsCount++;
                                    }
                                }
                            }                      
                        }
                    }
                    catch
                    {
                        Debug.LogError("Unable to get transforms on object named : '" + objs[i].name + "'");
                    }
                }
            }
            finally
            {
                //End the progress bar and show the number of objects that have been updated.
                if (showProgressBar)
                {
                    EditorUtility.ClearProgressBar();
                }

                if (changedObjectsCount > 0)
                {
                    if (changedObjectsCount == 1)
                    {
                        Debug.LogFormat("1 GameObject renamed");
                        consoleOutput = "1 GameObject renamed";
                    }
                    else
                    {
                        Debug.LogFormat("{0} GameObjects renamed", changedObjectsCount);
                        consoleOutput = string.Format("{0} GameObjects renamed", changedObjectsCount);
                    }
                }
                else
                {
                    Debug.LogFormat("No GameObjects renamed");
                    consoleOutput = "No GameObjects renamed";
                }
            }
        }
        #endregion

        #region PrefabControl

        /// <summary>
        /// These are the functions that perform the prefabs tool actions.
        /// </summary>

        static void CreatePrefab()
        {
            GameObject[] objs = Selection.gameObjects;

            int numberOfTransforms = objs.Length;
            bool showProgressBar = numberOfTransforms >= SelectionThresholdForProgressBar;
            int changedObjectsCount = 0;

            try
            {
                for (int i = 0; i < numberOfTransforms; i++)
                {

                    if (showProgressBar)
                    {
                        EditorUtility.DisplayProgressBar("Create prefabs", "Creating prefabs (" + i + "/" + numberOfTransforms + ")",
                            (float)i / (float)numberOfTransforms);
                    }

                    //We can filter our selection to only create prefabs for items that match our search
                    if (useSearch)
                    {
                        bool matchedSearch = false;

                        if (sType == SEARCHTYPE.NAME)
                        {
                            if (searchCaseSentive)
                            {
                                switch (op)
                                {
                                    case SEARCHOPTIONS.STARTS_WITH:
                                        if (objs[i].name.StartsWith(searchString))
                                        {
                                            matchedSearch = true;
                                        }
                                        break;

                                    case SEARCHOPTIONS.CONTAINS:
                                        if (objs[i].name.Contains(searchString))
                                        {
                                            matchedSearch = true;
                                        }
                                        break;

                                    case SEARCHOPTIONS.ENDS_WITH:
                                        if (objs[i].name.EndsWith(searchString))
                                        {
                                            matchedSearch = true;
                                        }
                                        break;
                                }
                            }
                            else
                            {
                                switch (op)
                                {
                                    case SEARCHOPTIONS.STARTS_WITH:
                                        if (objs[i].name.ToUpper().StartsWith(searchString.ToUpper()))
                                        {
                                            matchedSearch = true;
                                        }
                                        break;

                                    case SEARCHOPTIONS.CONTAINS:
                                        if (objs[i].name.ToUpper().Contains(searchString.ToUpper()))
                                        {
                                            matchedSearch = true;
                                        }
                                        break;

                                    case SEARCHOPTIONS.ENDS_WITH:
                                        if (objs[i].name.ToUpper().EndsWith(searchString.ToUpper()))
                                        {
                                            matchedSearch = true;
                                        }
                                        break;
                                }
                            }
                        }

                        if (sType == SEARCHTYPE.TAG)
                        {
                            if (searchTagCaseSentive)
                            {
                                if (objs[i].tag.ToUpper().Equals(searchTag.ToUpper()))
                                {
                                    matchedSearch = true;
                                }
                            }
                            else
                            {
                                if (objs[i].CompareTag(searchTag))
                                {
                                    matchedSearch = true;
                                }
                            }
                        }

                        if (sType == SEARCHTYPE.LAYER)
                        {
                            if (objs[i].gameObject.layer == searchLayer)
                            {
                                matchedSearch = true;
                            }
                        }
                                                

                        //We have matched our search
                        if (matchedSearch == true)
                        {
                            var go = objs[i];

                            string localPath = newPrefabsLocation + "/" + go.name + ".prefab";

                            //This will attempt to create the folder if it does not exist
                            localPath = FixPathAndCreateFolders(localPath);

                            if (AssetDatabase.LoadAssetAtPath(localPath, typeof(GameObject)))
                            {
                                if (EditorUtility.DisplayDialog("Are you sure?",
                                    "The prefab already exists. Do you want to overwrite it?",
                                    "Yes",
                                    "No"))
                                {
                                    CreateNew(go, localPath);
                                }
                            }
                            else
                            {
                                Debug.Log(go.name + " Prefab Created");
                                CreateNew(go, localPath);
                            }

                            changedObjectsCount++;
                        }
                    }
                    else
                    {

                        //No search so create prefabs of everything
                        var go = objs[i];

                        string localPath = newPrefabsLocation + "/" + go.name + ".prefab";

                        localPath = FixPathAndCreateFolders(localPath);

                        if (AssetDatabase.LoadAssetAtPath(localPath, typeof(GameObject)))
                        {
                            if (EditorUtility.DisplayDialog("Are you sure?",
                                "The prefab already exists. Do you want to overwrite it?",
                                "Yes",
                                "No"))
                            {
                                CreateNew(go, localPath);
                            }
                        }
                        else
                        {
                            Debug.Log(go.name + " Prefab Created");
                            CreateNew(go, localPath);
                        }

                        changedObjectsCount++;
                    }
                }
            }
            finally
            {
                if (showProgressBar)
                {
                    EditorUtility.ClearProgressBar();
                }

                if (changedObjectsCount > 0)
                {
                    if (changedObjectsCount == 1)
                    {
                        Debug.LogFormat("1 Prefab created");
                        consoleOutput = "1 Prefab created";
                    }
                    else
                    {
                        Debug.LogFormat("{0} Prefabs created", changedObjectsCount);
                        consoleOutput = string.Format("{0} Prefabs created", changedObjectsCount);
                    }
                }
                else
                {
                    Debug.LogFormat("No changes made");
                    consoleOutput = "No changes made";
                }
            }

        }

        public static string FixPathAndCreateFolders(string localPath)
        {
            //Format the string so as to remove junk and be able to work with it.            
            localPath = localPath.Replace('/', '\\');

            //so we need to simply split the localpath into an array, separated by slashes, and without blank spaces.
            string[] path = localPath.Split(new string[] { "\\" }, System.StringSplitOptions.RemoveEmptyEntries);
            string filename = path[path.Length - 1];

            string currentFolder = string.Empty;
            string finalFolder = "Assets";

            //Make sure that assets is always the first folder
            if (path[0].ToString().ToUpper().Equals("ASSETS"))
            {
                //add the rest of the path to the final folder, ignore the first one
                for (int i = 1; i < path.Length - 1; i++)
                {
                    finalFolder = finalFolder + "/" + path[i];
                }
            }
            else
            {
                //add all of the path to the final folder.
                for (int i = 0; i < path.Length - 1; i++)
                {
                    finalFolder = finalFolder + "/" + path[i];
                }
            }

            //If the final folder does not exist, we need to loop through the folders one by one creating them as we go along.
            if (!AssetDatabase.IsValidFolder(finalFolder))
            {
                int StartIndex = 0;

                if (!path[0].ToString().ToUpper().Equals("ASSETS"))
                {
                    currentFolder = "Assets";
                }
                else
                {
                    currentFolder = path[0].ToString();
                    StartIndex = 1;
                }

                //then we need to check if each sub folder exists, and if not, create it.
                for (int i = StartIndex; i < path.Length - 1; i++)
                {
                    string parentFolder = currentFolder;
                    currentFolder += "/" + path[i];
                     /*
                     * e.g. assets/prefabs
                     * e.g. assets/prefabs/temp
                     * e.g. assets/prefabs/temp/New
                     */

                    if (!AssetDatabase.IsValidFolder(currentFolder))
                    {
                        AssetDatabase.CreateFolder(parentFolder, path[i]);
                    }
                }
            }

            //Take our final folder path and add our file name. This is the new path.
            localPath = finalFolder + "/" + filename;
            return localPath;
        }

        //Creates the new prefab at the desired path
        static void CreateNew(GameObject obj, string localPath)
        {            
            UnityEngine.Object prefab = PrefabUtility.CreateEmptyPrefab(localPath);
            PrefabUtility.ReplacePrefab(obj, prefab, ReplacePrefabOptions.ConnectToPrefab);
        }

        //Apply prefab changes
        private static void ApplyPrefabs()
        {
            SearchPrefabConnections(ApplyToSelectedPrefabs);
        }

        //Revert prefab changes
        private static void ResetPrefabs()
        {
            SearchPrefabConnections(RevertToSelectedPrefabs);
        }

        //Change our prefab
        private static void SearchPrefabConnections(ChangePrefab changePrefabAction)
        {
            GameObject[] selectedTransforms = Selection.gameObjects;
            int numberOfTransforms = selectedTransforms.Length;
            bool showProgressBar = numberOfTransforms >= SelectionThresholdForProgressBar;
            int changedObjectsCount = 0;
            //Iterate through all the selected gameobjects
            try
            {
                for (int i = 0; i < numberOfTransforms; i++)
                {
                    if (showProgressBar)
                    {
                        EditorUtility.DisplayProgressBar("Update prefabs", "Updating prefabs (" + i + "/" + numberOfTransforms + ")",
                            (float)i / (float)numberOfTransforms);
                    }

                    var go = selectedTransforms[i];
                    var prefabType = PrefabUtility.GetPrefabType(go);
                    //Is the selected gameobject a prefab?                    
                    if (prefabType == PrefabType.PrefabInstance || prefabType == PrefabType.DisconnectedPrefabInstance)
                    {
                        var prefabRoot = PrefabUtility.FindRootGameObjectWithSameParentPrefab(go);
                        if (prefabRoot == null)
                        {
                            continue;
                        }

                        changePrefabAction(prefabRoot);
                        changedObjectsCount++;
                    }
                }
            }
            finally
            {
                if (showProgressBar)
                {
                    EditorUtility.ClearProgressBar();
                }
                Debug.LogFormat("{0} Prefab(s) updated", changedObjectsCount);
                consoleOutput = string.Format("{0} Prefabs(s) updated", changedObjectsCount);
            }
        }

        //Apply changes 
        private static void ApplyToSelectedPrefabs(GameObject go)
        {
            var prefabAsset = PrefabUtility.GetCorrespondingObjectFromSource(go);
            if (prefabAsset == null)
            {
                return;
            }
            PrefabUtility.ReplacePrefab(go, prefabAsset, ReplacePrefabOptions.ConnectToPrefab);
        }

        //Revert changes
        private static void RevertToSelectedPrefabs(GameObject go)
        {
            PrefabUtility.ReconnectToLastPrefab(go);
            PrefabUtility.RevertPrefabInstance(go);
        }

        #endregion

        #region ColliderControl

        /// <summary>
        /// These are the functions that perform the collider tool actions.
        /// </summary>

        static void DisableColliders()
        {
            //Our full selection
            GameObject[] objs = Selection.gameObjects;            
            int numberOfTransforms = objs.Length;
            bool showProgressBar = numberOfTransforms >= SelectionThresholdForProgressBar;
            int changedObjectsCount = 0;

            try
            {
                //loop through collection
                for (int i = 0; i < numberOfTransforms; i++)
                {

                    if (showProgressBar)
                    {
                        EditorUtility.DisplayProgressBar("Disable colliders", "Disabling colliders (" + i + "/" + numberOfTransforms + " parent objects)",
                            (float)i / (float)numberOfTransforms);
                    }

                    Transform[] allTransforms;

                    //do we include the children in this action?
                    if (includeChildrenColliders)
                    {
                        allTransforms = objs[i].GetComponentsInChildren<Transform>();
                    }
                    else
                    {
                        allTransforms = objs[i].GetComponents<Transform>();
                    }


                    foreach (Transform childTransform in allTransforms)
                    {
                        if (useSearch)
                        {
                            bool matchedSearch = false;

                            if (sType == SEARCHTYPE.NAME)
                            {
                                if (searchCaseSentive)
                                {
                                    switch (op)
                                    {
                                        case SEARCHOPTIONS.STARTS_WITH:
                                            if (childTransform.name.StartsWith(searchString))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;

                                        case SEARCHOPTIONS.CONTAINS:
                                            if (childTransform.name.Contains(searchString))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;

                                        case SEARCHOPTIONS.ENDS_WITH:
                                            if (childTransform.name.EndsWith(searchString))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;
                                    }
                                }
                                else
                                {
                                    switch (op)
                                    {
                                        case SEARCHOPTIONS.STARTS_WITH:
                                            if (childTransform.name.ToUpper().StartsWith(searchString.ToUpper()))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;

                                        case SEARCHOPTIONS.CONTAINS:
                                            if (childTransform.name.ToUpper().Contains(searchString.ToUpper()))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;

                                        case SEARCHOPTIONS.ENDS_WITH:
                                            if (childTransform.name.ToUpper().EndsWith(searchString.ToUpper()))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;
                                    }
                                }
                            }

                            if (sType == SEARCHTYPE.TAG)
                            {
                                if (searchTagCaseSentive)
                                {
                                    if (childTransform.tag.ToUpper().Equals(searchTag.ToUpper()))
                                    {
                                        matchedSearch = true;
                                    }
                                }
                                else
                                {
                                    if (childTransform.CompareTag(searchTag))
                                    {
                                        matchedSearch = true;
                                    }
                                }
                            }

                            if (sType == SEARCHTYPE.LAYER)
                            {
                                if (childTransform.gameObject.layer == searchLayer)
                                {
                                    matchedSearch = true;
                                }
                            }

                            //We need to only use go as the game object if it either matches the seach, or else do everything
                            if (matchedSearch == true)
                            {
                                try
                                {
                                    Collider[] allColliders;
                                    //try to get all colliders on the current object, in case there are multiple occurences
                                    allColliders = childTransform.GetComponents<Collider>();

                                    //we now have all the child colliders
                                    if (allColliders.Length > 0)
                                    {
                                        foreach (Collider aChild in allColliders)
                                        {
                                            //Record undo statement. Disable colldier.
                                            Undo.RecordObject(aChild, "Disable Collider(s)");
                                            aChild.enabled = false;
                                            changedObjectsCount++;
                                        }
                                    }
                                }
                                catch
                                {
                                    Debug.LogError("Unable to get colliders on object named : '" + childTransform.name + "'");
                                }
                            }
                        }
                        else
                        {
                            //No search filtering
                            try
                            {
                                Collider[] allColliders;
                                allColliders = childTransform.GetComponents<Collider>();

                                //we now have all the child colliders
                                if (allColliders.Length > 0)
                                {
                                    foreach (Collider aChild in allColliders)
                                    {
                                        //Undo statement. Disable colliders
                                        Undo.RecordObject(aChild, "Disable Collider(s)");
                                        aChild.enabled = false;
                                        changedObjectsCount++;
                                    }
                                }
                            }
                            catch
                            {
                                Debug.LogError("Unable to get colliders on object named : '" + childTransform.name + "'");
                            }
                        }
                    }
                }
            }
            finally
            {
                if (showProgressBar)
                {
                    EditorUtility.ClearProgressBar();
                }

                if (changedObjectsCount > 0)
                {
                    if (changedObjectsCount == 1)
                    {
                        Debug.LogFormat("1 collider disabled");
                        consoleOutput = "1 collider disabled";
                    }
                    else
                    {
                        Debug.LogFormat("{0} colliders disabled", changedObjectsCount);
                        consoleOutput = string.Format("{0} colliders disabled", changedObjectsCount);
                    }
                }
                else
                {
                    Debug.LogFormat("No changes made");
                    consoleOutput = "No changes made";
                }
            }
        }

        static void EnableColliders()
        {
            GameObject[] objs = Selection.gameObjects;
            //Here we have the gameobjects selected.
            int numberOfTransforms = objs.Length;
            bool showProgressBar = numberOfTransforms >= SelectionThresholdForProgressBar;
            int changedObjectsCount = 0;

            try
            {
                for (int i = 0; i < numberOfTransforms; i++)
                {

                    if (showProgressBar)
                    {
                        EditorUtility.DisplayProgressBar("Enable colliders", "Enabling colliders (" + i + "/" + numberOfTransforms + " parent objects)",
                            (float)i / (float)numberOfTransforms);
                    }

                    Transform[] allTransforms;

                    //Do we include children?
                    if (includeChildrenColliders)
                    {
                        allTransforms = objs[i].GetComponentsInChildren<Transform>();
                    }
                    else
                    {
                        allTransforms = objs[i].GetComponents<Transform>();
                    }


                    foreach (Transform childTransform in allTransforms)
                    {
                        if (useSearch)
                        {
                            bool matchedSearch = false;

                            if (sType == SEARCHTYPE.NAME)
                            {
                                if (searchCaseSentive)
                                {
                                    switch (op)
                                    {
                                        case SEARCHOPTIONS.STARTS_WITH:
                                            if (childTransform.name.StartsWith(searchString))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;

                                        case SEARCHOPTIONS.CONTAINS:
                                            if (childTransform.name.Contains(searchString))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;

                                        case SEARCHOPTIONS.ENDS_WITH:
                                            if (childTransform.name.EndsWith(searchString))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;
                                    }
                                }
                                else
                                {
                                    switch (op)
                                    {
                                        case SEARCHOPTIONS.STARTS_WITH:
                                            if (childTransform.name.ToUpper().StartsWith(searchString.ToUpper()))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;

                                        case SEARCHOPTIONS.CONTAINS:
                                            if (childTransform.name.ToUpper().Contains(searchString.ToUpper()))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;

                                        case SEARCHOPTIONS.ENDS_WITH:
                                            if (childTransform.name.ToUpper().EndsWith(searchString.ToUpper()))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;
                                    }
                                }
                            }

                            if (sType == SEARCHTYPE.TAG)
                            {
                                if (searchTagCaseSentive)
                                {
                                    if (childTransform.tag.ToUpper().Equals(searchTag.ToUpper()))
                                    {
                                        matchedSearch = true;
                                    }
                                }
                                else
                                {
                                    if (childTransform.CompareTag(searchTag))
                                    {
                                        matchedSearch = true;
                                    }
                                }
                            }

                            if (sType == SEARCHTYPE.LAYER)
                            {
                                if (childTransform.gameObject.layer == searchLayer)
                                {
                                    matchedSearch = true;
                                }
                            }

                            //We need to only use go as the game object if it either matches the seach, or else do everything
                            if (matchedSearch == true)
                            {
                                try
                                {
                                    Collider[] allColliders;
                                    allColliders = childTransform.GetComponents<Collider>();

                                    //we now have all the child colliders
                                    if (allColliders.Length > 0)
                                    {
                                        foreach (Collider aChild in allColliders)
                                        {
                                            //Record undo. Enable colliders.
                                            Undo.RecordObject(aChild, "Enable Collider(s)");
                                            aChild.enabled = true;
                                            changedObjectsCount++;
                                        }
                                    }
                                }
                                catch
                                {
                                    Debug.LogError("Unable to get colliders on object named : '" + childTransform.name + "'");
                                }
                            }
                        }
                        else
                        {                          
                            try
                            {
                                Collider[] allColliders;
                                allColliders = childTransform.GetComponents<Collider>();

                                //we now have all the child colliders
                                if (allColliders.Length > 0)
                                {
                                    foreach (Collider aChild in allColliders)
                                    {
                                        //Record undo. Enable Colliders
                                        Undo.RecordObject(aChild, "Enable Collider(s)");
                                        aChild.enabled = true;
                                        changedObjectsCount++;
                                    }
                                }
                            }
                            catch
                            {
                                Debug.LogError("Unable to get colliders on object named : '" + childTransform.name + "'");
                            }
                        }
                    }
                }
            }
            finally
            {
                if (showProgressBar)
                {
                    EditorUtility.ClearProgressBar();
                }

                if (changedObjectsCount > 0)
                {
                    if (changedObjectsCount == 1)
                    {
                        Debug.LogFormat("1 collider enabled");
                        consoleOutput = "1 collider enabled";
                    }
                    else
                    {
                        Debug.LogFormat("{0} colliders enabled", changedObjectsCount);
                        consoleOutput = string.Format("{0} colliders enabled", changedObjectsCount);
                    }
                }
                else
                {
                    Debug.LogFormat("No changes made");
                    consoleOutput = "No changes made";
                }

            }
        }

        static void DisableIsTrigger()
        {
            GameObject[] objs = Selection.gameObjects;
            //Here we have the gameobjects selected.
            int numberOfTransforms = objs.Length;
            bool showProgressBar = numberOfTransforms >= SelectionThresholdForProgressBar;
            int changedObjectsCount = 0;

            try
            {
                for (int i = 0; i < numberOfTransforms; i++)
                {

                    if (showProgressBar)
                    {
                        EditorUtility.DisplayProgressBar("Disable isTrigger", "Disabling isTrigger (" + i + "/" + numberOfTransforms + " parent objects)",
                            (float)i / (float)numberOfTransforms);
                    }

                    Transform[] allTransforms;

                    if (includeChildrenColliders)
                    {
                        allTransforms = objs[i].GetComponentsInChildren<Transform>();
                    }
                    else
                    {
                        allTransforms = objs[i].GetComponents<Transform>();
                    }


                    foreach (Transform childTransform in allTransforms)
                    {

                        if (useSearch)
                        {
                            bool matchedSearch = false;

                            if (sType == SEARCHTYPE.NAME)
                            {
                                if (searchCaseSentive)
                                {
                                    switch (op)
                                    {
                                        case SEARCHOPTIONS.STARTS_WITH:
                                            if (childTransform.name.StartsWith(searchString))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;

                                        case SEARCHOPTIONS.CONTAINS:
                                            if (childTransform.name.Contains(searchString))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;

                                        case SEARCHOPTIONS.ENDS_WITH:
                                            if (childTransform.name.EndsWith(searchString))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;
                                    }
                                }
                                else
                                {
                                    switch (op)
                                    {
                                        case SEARCHOPTIONS.STARTS_WITH:
                                            if (childTransform.name.ToUpper().StartsWith(searchString.ToUpper()))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;

                                        case SEARCHOPTIONS.CONTAINS:
                                            if (childTransform.name.ToUpper().Contains(searchString.ToUpper()))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;

                                        case SEARCHOPTIONS.ENDS_WITH:
                                            if (childTransform.name.ToUpper().EndsWith(searchString.ToUpper()))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;
                                    }
                                }
                            }

                            if (sType == SEARCHTYPE.TAG)
                            {
                                if (searchTagCaseSentive)
                                {
                                    if (childTransform.tag.ToUpper().Equals(searchTag.ToUpper()))
                                    {
                                        matchedSearch = true;
                                    }
                                }
                                else
                                {
                                    if (childTransform.CompareTag(searchTag))
                                    {
                                        matchedSearch = true;
                                    }
                                }
                            }

                            if (sType == SEARCHTYPE.LAYER)
                            {
                                if (childTransform.gameObject.layer == searchLayer)
                                {
                                    matchedSearch = true;
                                }
                            }

                            //We need to only use go as the game object if it either matches the seach, or else do everything
                            if (matchedSearch == true)
                            {
                                try
                                {
                                    Collider[] allColliders;
                                    allColliders = childTransform.GetComponents<Collider>();

                                    //we now have all the child colliders
                                    if (allColliders.Length > 0)
                                    {
                                        foreach (Collider aChild in allColliders)
                                        {
                                            //Record undo. Disable isTrigger
                                            Undo.RecordObject(aChild, "Disable isTrigger");
                                            aChild.isTrigger = false;
                                            changedObjectsCount++;
                                        }
                                    }
                                }
                                catch
                                {
                                    Debug.LogError("Unable to get colliders on object named : '" + childTransform.name + "'");
                                }
                            }
                        }
                        else
                        {                           
                            try
                            {
                                Collider[] allColliders;
                                allColliders = childTransform.GetComponents<Collider>();

                                //we now have all the child colliders
                                if (allColliders.Length > 0)
                                {
                                    foreach (Collider aChild in allColliders)
                                    {
                                        //Record Undo. Disable isTrigger
                                        Undo.RecordObject(aChild, "Disable isTrigger");
                                        aChild.isTrigger = false;
                                        changedObjectsCount++;
                                    }
                                }
                            }
                            catch
                            {
                                Debug.LogError("Unable to get colliders on object named : '" + childTransform.name + "'");
                            }
                        }
                    }
                }
            }
            finally
            {
                if (showProgressBar)
                {
                    EditorUtility.ClearProgressBar();
                }

                if (changedObjectsCount > 0)
                {
                    if (changedObjectsCount == 1)
                    {
                        Debug.LogFormat("isTrigger disabled on 1 collider");
                        consoleOutput = "isTrigger disabled on 1 collider";
                    }
                    else
                    {
                        Debug.LogFormat("isTrigger disabled on {0} colliders", changedObjectsCount);
                        consoleOutput = string.Format("isTrigger disabled on {0} colliders", changedObjectsCount);
                    }
                }
                else
                {
                    Debug.LogFormat("No changes made");
                    consoleOutput = "No changes made";
                }
            }
        }

        static void EnableIsTrigger()
        {
            try
            {
                EnableConvex();
            }
            catch
            {
                    //do nothing because we handle errors internally and dont want to do anything if there are no convex meshes.
            }

            GameObject[] objs = Selection.gameObjects;
            //Here we have the gameobjects selected.
            int numberOfTransforms = objs.Length;
            bool showProgressBar = numberOfTransforms >= SelectionThresholdForProgressBar;
            int changedObjectsCount = 0;

            try
            {
                for (int i = 0; i < numberOfTransforms; i++)
                {

                    if (showProgressBar)
                    {
                        EditorUtility.DisplayProgressBar("Enable isTrigger", "Enabling isTrigger (" + i + "/" + numberOfTransforms + " parent objects)",
                            (float)i / (float)numberOfTransforms);
                    }

                    Transform[] allTransforms;

                    //Do we include children?
                    if (includeChildrenColliders)
                    {
                        allTransforms = objs[i].GetComponentsInChildren<Transform>();
                    }
                    else
                    {
                        allTransforms = objs[i].GetComponents<Transform>();
                    }


                    foreach (Transform childTransform in allTransforms)
                    {
                        if (useSearch)
                        {
                            bool matchedSearch = false;

                            if (sType == SEARCHTYPE.NAME)
                            {
                                if (searchCaseSentive)
                                {
                                    switch (op)
                                    {
                                        case SEARCHOPTIONS.STARTS_WITH:
                                            if (childTransform.name.StartsWith(searchString))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;

                                        case SEARCHOPTIONS.CONTAINS:
                                            if (childTransform.name.Contains(searchString))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;

                                        case SEARCHOPTIONS.ENDS_WITH:
                                            if (childTransform.name.EndsWith(searchString))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;
                                    }
                                }
                                else
                                {
                                    switch (op)
                                    {
                                        case SEARCHOPTIONS.STARTS_WITH:
                                            if (childTransform.name.ToUpper().StartsWith(searchString.ToUpper()))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;

                                        case SEARCHOPTIONS.CONTAINS:
                                            if (childTransform.name.ToUpper().Contains(searchString.ToUpper()))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;

                                        case SEARCHOPTIONS.ENDS_WITH:
                                            if (childTransform.name.ToUpper().EndsWith(searchString.ToUpper()))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;
                                    }
                                }
                            }

                            if (sType == SEARCHTYPE.TAG)
                            {
                                if (searchTagCaseSentive)
                                {
                                    if (childTransform.tag.ToUpper().Equals(searchTag.ToUpper()))
                                    {
                                        matchedSearch = true;
                                    }
                                }
                                else
                                {
                                    if (childTransform.CompareTag(searchTag))
                                    {
                                        matchedSearch = true;
                                    }
                                }
                            }

                            if (sType == SEARCHTYPE.LAYER)
                            {
                                if (childTransform.gameObject.layer == searchLayer)
                                {
                                    matchedSearch = true;
                                }
                            }

                            //We need to only use go as the game object if it either matches the seach, or else do everything
                            if (matchedSearch == true)
                            {
                                try
                                {
                                    Collider[] allColliders;
                                    allColliders = childTransform.GetComponents<Collider>();

                                    //we now have all the child colliders
                                    if (allColliders.Length > 0)
                                    {
                                        foreach (Collider aChild in allColliders)
                                        {
                                            //Record undo statement. Enable isTrigger
                                            Undo.RecordObject(aChild, "Enable isTrigger");
                                            aChild.isTrigger = true;
                                            changedObjectsCount++;
                                        }
                                    }
                                }
                                catch
                                {
                                    Debug.LogError("Unable to get colliders on object named : '" + childTransform.name + "'");
                                }
                            }
                        }
                        else
                        {                         
                            try
                            {
                                Collider[] allColliders;
                                allColliders = childTransform.GetComponents<Collider>();

                                //we now have all the child colliders
                                if (allColliders.Length > 0)
                                {
                                    foreach (Collider aChild in allColliders)
                                    {
                                        //Record Undo. Enable isTrigger.
                                        Undo.RecordObject(aChild, "Enable isTrigger");
                                        aChild.isTrigger = true;
                                        changedObjectsCount++;
                                    }
                                }
                            }
                            catch
                            {
                                Debug.LogError("Unable to get colliders on object named : '" + childTransform.name + "'");
                            }
                        }
                    }
                }
            }
            finally
            {
                if (showProgressBar)
                {
                    EditorUtility.ClearProgressBar();
                }

                if (changedObjectsCount > 0)
                {
                    if (changedObjectsCount == 1)
                    {
                        Debug.LogFormat("isTrigger enabled on 1 collider");
                        consoleOutput = "isTrigger enabled on 1 collider";
                    }
                    else
                    {
                        Debug.LogFormat("isTrigger enabled on {0} colliders", changedObjectsCount);
                        consoleOutput = string.Format("isTrigger enabled on {0} colliders", changedObjectsCount);
                    }
                }
                else
                {
                    Debug.LogFormat("No changes made");
                    consoleOutput = "No changes made";
                }
            }
        }

        static void DisableConvex()
        {
            try
            {
                DisableIsTrigger();
            }
            catch
            {
                //do nothing because we handle errors internally and dont want to do anything if there are no convex meshes.
            }
            
            GameObject[] objs = Selection.gameObjects;
            //Here we have the gameobjects selected.
            int numberOfTransforms = objs.Length;
            bool showProgressBar = numberOfTransforms >= SelectionThresholdForProgressBar;
            int changedObjectsCount = 0;

            try
            {
                for (int i = 0; i < numberOfTransforms; i++)
                {

                    if (showProgressBar)
                    {
                        EditorUtility.DisplayProgressBar("Disable Convex", "Disabling Convex (" + i + "/" + numberOfTransforms + " parent objects)",
                            (float)i / (float)numberOfTransforms);
                    }

                    Transform[] allTransforms;

                    if (includeChildrenColliders)
                    {
                        allTransforms = objs[i].GetComponentsInChildren<Transform>();
                    }
                    else
                    {
                        allTransforms = objs[i].GetComponents<Transform>();
                    }


                    foreach (Transform childTransform in allTransforms)
                    {

                        if (useSearch)
                        {
                            bool matchedSearch = false;

                            if (sType == SEARCHTYPE.NAME)
                            {
                                if (searchCaseSentive)
                                {
                                    switch (op)
                                    {
                                        case SEARCHOPTIONS.STARTS_WITH:
                                            if (childTransform.name.StartsWith(searchString))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;

                                        case SEARCHOPTIONS.CONTAINS:
                                            if (childTransform.name.Contains(searchString))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;

                                        case SEARCHOPTIONS.ENDS_WITH:
                                            if (childTransform.name.EndsWith(searchString))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;
                                    }
                                }
                                else
                                {
                                    switch (op)
                                    {
                                        case SEARCHOPTIONS.STARTS_WITH:
                                            if (childTransform.name.ToUpper().StartsWith(searchString.ToUpper()))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;

                                        case SEARCHOPTIONS.CONTAINS:
                                            if (childTransform.name.ToUpper().Contains(searchString.ToUpper()))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;

                                        case SEARCHOPTIONS.ENDS_WITH:
                                            if (childTransform.name.ToUpper().EndsWith(searchString.ToUpper()))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;
                                    }
                                }
                            }

                            if (sType == SEARCHTYPE.TAG)
                            {
                                if (searchTagCaseSentive)
                                {
                                    if (childTransform.tag.ToUpper().Equals(searchTag.ToUpper()))
                                    {
                                        matchedSearch = true;
                                    }
                                }
                                else
                                {
                                    if (childTransform.CompareTag(searchTag))
                                    {
                                        matchedSearch = true;
                                    }
                                }
                            }

                            if (sType == SEARCHTYPE.LAYER)
                            {
                                if (childTransform.gameObject.layer == searchLayer)
                                {
                                    matchedSearch = true;
                                }
                            }

                            //We need to only use go as the game object if it either matches the seach, or else do everything
                            if (matchedSearch == true)
                            {
                                try
                                {
                                    Collider[] allColliders;
                                    allColliders = childTransform.GetComponents<Collider>();

                                    //we now have all the child colliders
                                    if (allColliders.Length > 0)
                                    {
                                        foreach (Collider aChild in allColliders)
                                        {
                                            if (aChild.GetType() == typeof(MeshCollider))
                                            {
                                                //Record undo statement. Enable isTrigger
                                                Undo.RecordObject(aChild, "Disable Convex");
                                                MeshCollider newChild = (MeshCollider)aChild;
                                                newChild.convex = false;
                                                changedObjectsCount++;
                                            }
                                        }                                        
                                    }
                                }
                                catch
                                {
                                    Debug.LogError("Unable to get colliders on object named : '" + childTransform.name + "'");
                                }
                            }
                        }
                        else
                        {
                            try
                            {
                                Collider[] allColliders;
                                allColliders = childTransform.GetComponents<Collider>();

                                //we now have all the child colliders
                                if (allColliders.Length > 0)
                                {
                                    foreach (MeshCollider aChild in allColliders)
                                    {
                                        //Record Undo. Disable isTrigger
                                        Undo.RecordObject(aChild, "Disable Convex");                                        
                                        aChild.convex = false;
                                        changedObjectsCount++;
                                    }
                                }
                            }
                            catch
                            {
                                Debug.LogError("Unable to get colliders on object named : '" + childTransform.name + "'");
                            }
                        }
                    }
                }
            }
            finally
            {
                if (showProgressBar)
                {
                    EditorUtility.ClearProgressBar();
                }

                if (changedObjectsCount > 0)
                {
                    if (changedObjectsCount == 1)
                    {
                        Debug.LogFormat("Convex disabled on 1 collider");
                        consoleOutput = "Convex disabled on 1 collider";
                    }
                    else
                    {
                        Debug.LogFormat("Convex disabled on {0} colliders", changedObjectsCount);
                        consoleOutput = string.Format("Convex disabled on {0} colliders", changedObjectsCount);
                    }
                }
                else
                {
                    Debug.LogFormat("No changes made");
                    consoleOutput = "No changes made";
                }
            }
        }

        static void EnableConvex()
        {
            GameObject[] objs = Selection.gameObjects;
            //Here we have the gameobjects selected.
            int numberOfTransforms = objs.Length;
            bool showProgressBar = numberOfTransforms >= SelectionThresholdForProgressBar;
            int changedObjectsCount = 0;

            try
            {
                for (int i = 0; i < numberOfTransforms; i++)
                {

                    if (showProgressBar)
                    {
                        EditorUtility.DisplayProgressBar("Enable Convex", "Enabling Convex (" + i + "/" + numberOfTransforms + " parent objects)",
                            (float)i / (float)numberOfTransforms);
                    }

                    Transform[] allTransforms;

                    //Do we include children?
                    if (includeChildrenColliders)
                    {
                        allTransforms = objs[i].GetComponentsInChildren<Transform>();
                    }
                    else
                    {
                        allTransforms = objs[i].GetComponents<Transform>();
                    }


                    foreach (Transform childTransform in allTransforms)
                    {
                        if (useSearch)
                        {
                            bool matchedSearch = false;

                            if (sType == SEARCHTYPE.NAME)
                            {
                                if (searchCaseSentive)
                                {
                                    switch (op)
                                    {
                                        case SEARCHOPTIONS.STARTS_WITH:
                                            if (childTransform.name.StartsWith(searchString))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;

                                        case SEARCHOPTIONS.CONTAINS:
                                            if (childTransform.name.Contains(searchString))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;

                                        case SEARCHOPTIONS.ENDS_WITH:
                                            if (childTransform.name.EndsWith(searchString))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;
                                    }
                                }
                                else
                                {
                                    switch (op)
                                    {
                                        case SEARCHOPTIONS.STARTS_WITH:
                                            if (childTransform.name.ToUpper().StartsWith(searchString.ToUpper()))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;

                                        case SEARCHOPTIONS.CONTAINS:
                                            if (childTransform.name.ToUpper().Contains(searchString.ToUpper()))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;

                                        case SEARCHOPTIONS.ENDS_WITH:
                                            if (childTransform.name.ToUpper().EndsWith(searchString.ToUpper()))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;
                                    }
                                }
                            }

                            if (sType == SEARCHTYPE.TAG)
                            {
                                if (searchTagCaseSentive)
                                {
                                    if (childTransform.tag.ToUpper().Equals(searchTag.ToUpper()))
                                    {
                                        matchedSearch = true;
                                    }
                                }
                                else
                                {
                                    if (childTransform.CompareTag(searchTag))
                                    {
                                        matchedSearch = true;
                                    }
                                }
                            }

                            if (sType == SEARCHTYPE.LAYER)
                            {
                                if (childTransform.gameObject.layer == searchLayer)
                                {
                                    matchedSearch = true;
                                }
                            }

                            //We need to only use go as the game object if it either matches the seach, or else do everything
                            if (matchedSearch == true)
                            {
                                try
                                {
                                    Collider[] allColliders;
                                    allColliders = childTransform.GetComponents<Collider>();

                                    //we now have all the child colliders
                                    if (allColliders.Length > 0)
                                    {
                                        foreach (Collider aChild in allColliders)
                                        {
                                            if(aChild.GetType() == typeof(MeshCollider))
                                            {
                                                //Record undo statement. Enable isTrigger
                                                Undo.RecordObject(aChild, "Enable Convex");
                                                MeshCollider newChild = (MeshCollider)aChild;
                                                newChild.convex = true;
                                                changedObjectsCount++;
                                            }                                            
                                        }
                                    }
                                }
                                catch
                                {
                                    Debug.LogError("Unable to get colliders on object named : '" + childTransform.name + "'");
                                }
                            }
                        }
                        else
                        {
                            try
                            {
                                Collider[] allColliders;
                                allColliders = childTransform.GetComponents<Collider>();

                                //we now have all the child colliders
                                if (allColliders.Length > 0)
                                {
                                    foreach (MeshCollider aChild in allColliders)
                                    {
                                        //Record Undo. Enable isTrigger.
                                        Undo.RecordObject(aChild, "Enable Convex");
                                        aChild.convex = true;
                                        changedObjectsCount++;
                                    }
                                }
                            }
                            catch
                            {
                                Debug.LogError("Unable to get colliders on object named : '" + childTransform.name + "'");
                            }
                        }
                    }
                }
            }
            finally
            {
                if (showProgressBar)
                {
                    EditorUtility.ClearProgressBar();
                }

                if (changedObjectsCount > 0)
                {
                    if (changedObjectsCount == 1)
                    {
                        Debug.LogFormat("Convex enabled on 1 collider");
                        consoleOutput = "Convex enabled on 1 collider";
                    }
                    else
                    {
                        Debug.LogFormat("Convex enabled on {0} colliders", changedObjectsCount);
                        consoleOutput = string.Format("Convex enabled on {0} colliders", changedObjectsCount);
                    }
                }
                else
                {
                    Debug.LogFormat("No changes made");
                    consoleOutput = "No changes made";
                }
            }
        }

        static void AddColliders(string ColliderType)
        {
            GameObject[] objs = Selection.gameObjects;
            //Here we have the gameobjects selected.
            int numberOfTransforms = objs.Length;
            bool showProgressBar = numberOfTransforms >= SelectionThresholdForProgressBar;
            int changedObjectsCount = 0;

            try
            {
                for (int i = 0; i < numberOfTransforms; i++)
                {

                    if (showProgressBar)
                    {
                        EditorUtility.DisplayProgressBar("Add " + ColliderType + " Collider", "Adding " + ColliderType + " Collider (" + i + "/" + numberOfTransforms + " parent objects)",
                            (float)i / (float)numberOfTransforms);
                    }


                    Transform[] allTransforms;

                    //Are we adding colliders to child objects?
                    if (includeChildrenColliders)
                    {
                        allTransforms = objs[i].GetComponentsInChildren<Transform>();
                    }
                    else
                    {
                        allTransforms = objs[i].GetComponents<Transform>();
                    }


                    foreach (Transform childTransform in allTransforms)
                    {
                        if (useSearch)
                        {
                            bool matchedSearch = false;

                            if (sType == SEARCHTYPE.NAME)
                            {
                                if (searchCaseSentive)
                                {
                                    switch (op)
                                    {
                                        case SEARCHOPTIONS.STARTS_WITH:
                                            if (childTransform.name.StartsWith(searchString))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;

                                        case SEARCHOPTIONS.CONTAINS:
                                            if (childTransform.name.Contains(searchString))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;

                                        case SEARCHOPTIONS.ENDS_WITH:
                                            if (childTransform.name.EndsWith(searchString))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;
                                    }
                                }
                                else
                                {
                                    switch (op)
                                    {
                                        case SEARCHOPTIONS.STARTS_WITH:
                                            if (childTransform.name.ToUpper().StartsWith(searchString.ToUpper()))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;

                                        case SEARCHOPTIONS.CONTAINS:
                                            if (childTransform.name.ToUpper().Contains(searchString.ToUpper()))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;

                                        case SEARCHOPTIONS.ENDS_WITH:
                                            if (childTransform.name.ToUpper().EndsWith(searchString.ToUpper()))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;
                                    }
                                }
                            }

                            if (sType == SEARCHTYPE.TAG)
                            {
                                if (searchTagCaseSentive)
                                {
                                    if (childTransform.tag.ToUpper().Equals(searchTag.ToUpper()))
                                    {
                                        matchedSearch = true;
                                    }
                                }
                                else
                                {
                                    if (childTransform.CompareTag(searchTag))
                                    {
                                        matchedSearch = true;
                                    }
                                }
                            }

                            if (sType == SEARCHTYPE.LAYER)
                            {
                                if (childTransform.gameObject.layer == searchLayer)
                                {
                                    matchedSearch = true;
                                }
                            }

                            //We need to only use go as the game object if it either matches the seach, or else do everything
                            if (matchedSearch == true)
                            {
                                try
                                {
                                    //Switch action based on collider type.
                                    switch (ColliderType)
                                    {
                                        case "Sphere":
                                            {
                                                var scriptToAdd = childTransform.gameObject.GetComponent(typeof(SphereCollider));
                                                //only add a sphere collider if one isn't already present.
                                                if (scriptToAdd == null)
                                                {
                                                    try
                                                    {
                                                        //Record undo statement. Add the collider.
                                                        Undo.AddComponent(childTransform.gameObject, typeof(SphereCollider));
                                                        changedObjectsCount++;
                                                    }
                                                    catch
                                                    {
                                                        Debug.LogError("Unable to add " + ColliderType + " collider to object named : '" + childTransform.name + "'");
                                                    }
                                                }
                                            }
                                            break;

                                        case "Box":
                                            {
                                                var scriptToAdd = childTransform.gameObject.GetComponent(typeof(BoxCollider));
                                                //only add a box collider if one isn't already present.
                                                if (scriptToAdd == null)
                                                {
                                                    try
                                                    {
                                                        //Record undo statement. Add the collider.
                                                        Undo.AddComponent(childTransform.gameObject, typeof(BoxCollider));
                                                        changedObjectsCount++;
                                                    }
                                                    catch
                                                    {
                                                        Debug.LogError("Unable to add " + ColliderType + " collider to object named : '" + childTransform.name + "'");
                                                    }
                                                }
                                            }
                                            break;

                                        case "Capsule":
                                            {
                                                var scriptToAdd = childTransform.gameObject.GetComponent(typeof(CapsuleCollider));
                                                //only add a capsule collider if one isn't already present.
                                                if (scriptToAdd == null)
                                                {
                                                    try
                                                    {
                                                        //Record undo statement. Add the collider.
                                                        Undo.AddComponent(childTransform.gameObject, typeof(CapsuleCollider));
                                                        changedObjectsCount++;
                                                    }
                                                    catch
                                                    {
                                                        Debug.LogError("Unable to add " + ColliderType + " collider to object named : '" + childTransform.name + "'");
                                                    }
                                                }
                                            }
                                            break;

                                        case "Mesh":
                                            {
                                                var scriptToAdd = childTransform.gameObject.GetComponent(typeof(MeshCollider));
                                                //only add a mesh collider if one isn't already present.
                                                if (scriptToAdd == null)
                                                {
                                                    try
                                                    {
                                                        //Record undo statement. Add the collider.
                                                        Undo.AddComponent(childTransform.gameObject, typeof(MeshCollider));
                                                        changedObjectsCount++;
                                                    }
                                                    catch
                                                    {
                                                        Debug.LogError("Unable to add " + ColliderType + " collider to object named : '" + childTransform.name + "'");
                                                    }
                                                }
                                            }
                                            break;
                                    }
                                }
                                catch
                                {
                                    Debug.LogError("Unable to add " + ColliderType + " collider to object named : '" + childTransform.name + "'");
                                }
                            }
                        }
                        else
                        {                            
                            try
                            {
                                //Switch action based on collider type.
                                switch (ColliderType)
                                {
                                    case "Sphere":
                                        {
                                            var scriptToAdd = childTransform.gameObject.GetComponent(typeof(SphereCollider));
                                            //only add a sphere collider if one isn't already present.
                                            if (scriptToAdd == null)
                                            {
                                                try
                                                {
                                                    //Record undo statement. Add the collider.
                                                    Undo.AddComponent(childTransform.gameObject, typeof(SphereCollider));
                                                    changedObjectsCount++;
                                                }
                                                catch
                                                {
                                                    Debug.LogError("Unable to add " + ColliderType + " collider to object named : '" + childTransform.name + "'");
                                                }
                                            }
                                        }
                                        break;

                                    case "Box":
                                        {
                                            var scriptToAdd = childTransform.gameObject.GetComponent(typeof(BoxCollider));
                                            //only add a sphere collider if one isn't already present.
                                            if (scriptToAdd == null)
                                            {
                                                try
                                                {
                                                    //Record undo statement. Add the collider.
                                                    Undo.AddComponent(childTransform.gameObject, typeof(BoxCollider));
                                                    changedObjectsCount++;
                                                }
                                                catch
                                                {
                                                    Debug.LogError("Unable to add " + ColliderType + " collider to object named : '" + childTransform.name + "'");
                                                }
                                            }
                                        }
                                        break;

                                    case "Capsule":
                                        {
                                            var scriptToAdd = childTransform.gameObject.GetComponent(typeof(CapsuleCollider));
                                            //only add a sphere collider if one isn't already present.
                                            if (scriptToAdd == null)
                                            {
                                                try
                                                {
                                                    //Record undo statement. Add the collider.
                                                    Undo.AddComponent(childTransform.gameObject, typeof(CapsuleCollider));
                                                    changedObjectsCount++;
                                                }
                                                catch
                                                {
                                                    Debug.LogError("Unable to add " + ColliderType + " collider to object named : '" + childTransform.name + "'");
                                                }
                                            }
                                        }
                                        break;

                                    case "Mesh":
                                        {
                                            var scriptToAdd = childTransform.gameObject.GetComponent(typeof(MeshCollider));
                                            //only add a sphere collider if one isn't already present.
                                            if (scriptToAdd == null)
                                            {
                                                try
                                                {
                                                    //Record undo statement. Add the collider.
                                                    Undo.AddComponent(childTransform.gameObject, typeof(MeshCollider));
                                                    changedObjectsCount++;
                                                }
                                                catch
                                                {
                                                    Debug.LogError("Unable to add " + ColliderType + " collider to object named : '" + childTransform.name + "'");
                                                }
                                            }
                                        }
                                        break;
                                }
                            }
                            catch
                            {
                                Debug.LogError("Unable to add " + ColliderType + " to object named : '" + childTransform.name + "'");
                            }
                        }
                    }
                }
            }
            finally
            {
                if (showProgressBar)
                {
                    EditorUtility.ClearProgressBar();
                }

                if (changedObjectsCount > 0)
                {
                    if (changedObjectsCount == 1)
                    {
                        Debug.LogFormat("1 " + ColliderType + " collider added");
                        consoleOutput = "1 " + ColliderType + " collider added";
                    }
                    else
                    {
                        Debug.LogFormat("{0} " + ColliderType + " colliders added", changedObjectsCount);
                        consoleOutput = string.Format("{0} " + ColliderType + " colliders added", changedObjectsCount);
                    }
                }
                else
                {
                    Debug.LogFormat("No " + ColliderType + " colliders added");
                    consoleOutput = "No " + ColliderType + " colliders added";
                }

            }
        }

        static void RemoveColliders(string ColliderType)
        {
            GameObject[] objs = Selection.gameObjects;
            //Here we have the gameobjects selected.
            int numberOfTransforms = objs.Length;
            bool showProgressBar = numberOfTransforms >= SelectionThresholdForProgressBar;
            int changedObjectsCount = 0;

            try
            {
                for (int i = 0; i < numberOfTransforms; i++)
                {

                    if (showProgressBar)
                    {
                        EditorUtility.DisplayProgressBar("Remove " + ColliderType + " Collider", "Removing " + ColliderType + " collider (" + i + "/" + numberOfTransforms + " parent objects)",
                            (float)i / (float)numberOfTransforms);
                    }

                    Transform[] allTransforms;

                    //Are we removing colliders from child objects?
                    if (includeChildrenColliders)
                    {
                        allTransforms = objs[i].GetComponentsInChildren<Transform>();
                    }
                    else
                    {
                        allTransforms = objs[i].GetComponents<Transform>();
                    }


                    foreach (Transform childTransform in allTransforms)
                    {                        
                        if (useSearch)
                        {
                            bool matchedSearch = false;

                            if (sType == SEARCHTYPE.NAME)
                            {
                                if (searchCaseSentive)
                                {
                                    switch (op)
                                    {
                                        case SEARCHOPTIONS.STARTS_WITH:
                                            if (childTransform.name.StartsWith(searchString))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;

                                        case SEARCHOPTIONS.CONTAINS:
                                            if (childTransform.name.Contains(searchString))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;

                                        case SEARCHOPTIONS.ENDS_WITH:
                                            if (childTransform.name.EndsWith(searchString))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;
                                    }
                                }
                                else
                                {
                                    switch (op)
                                    {
                                        case SEARCHOPTIONS.STARTS_WITH:
                                            if (childTransform.name.ToUpper().StartsWith(searchString.ToUpper()))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;

                                        case SEARCHOPTIONS.CONTAINS:
                                            if (childTransform.name.ToUpper().Contains(searchString.ToUpper()))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;

                                        case SEARCHOPTIONS.ENDS_WITH:
                                            if (childTransform.name.ToUpper().EndsWith(searchString.ToUpper()))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;
                                    }
                                }
                            }

                            if (sType == SEARCHTYPE.TAG)
                            {
                                if (searchTagCaseSentive)
                                {
                                    if (childTransform.tag.ToUpper().Equals(searchTag.ToUpper()))
                                    {
                                        matchedSearch = true;
                                    }
                                }
                                else
                                {
                                    if (childTransform.CompareTag(searchTag))
                                    {
                                        matchedSearch = true;
                                    }
                                }
                            }

                            if (sType == SEARCHTYPE.LAYER)
                            {
                                if (childTransform.gameObject.layer == searchLayer)
                                {
                                    matchedSearch = true;
                                }
                            }

                            //We need to only use go as the game object if it either matches the seach, or else do everything
                            if (matchedSearch == true)
                            {
                                try
                                {
                                    try
                                    {
                                        switch (ColliderType)
                                        {
                                            case "Sphere":
                                                {
                                                    var collidersToRemove = childTransform.gameObject.GetComponents(typeof(SphereCollider));
                                                    //find if we have one or multiple colliders to remove. If so, we remove them all if they match the type.
                                                    foreach (SphereCollider aComp in collidersToRemove)
                                                    {
                                                        if (aComp != null)
                                                        {
                                                            if (childTransform.gameObject.CanDestroy(aComp.GetType()))
                                                            {
                                                                //Record Undo statement. Remove collider.
                                                                Undo.DestroyObjectImmediate(aComp);
                                                                changedObjectsCount++;
                                                            }
                                                            else
                                                            {
                                                                Debug.Log("Unable to remove SphereCollider on object named : " + childTransform.name + " due to dependencies");
                                                            }

                                                        }
                                                    }
                                                }
                                                break;

                                            case "Box":
                                                {
                                                    var collidersToRemove = childTransform.gameObject.GetComponents(typeof(BoxCollider));
                                                    //find if we have one or multiple colliders to remove. If so, we remove them all if they match the type.
                                                    foreach (BoxCollider aComp in collidersToRemove)
                                                    {
                                                        if (aComp != null)
                                                        {
                                                            if (childTransform.gameObject.CanDestroy(aComp.GetType()))
                                                            {
                                                                //Record Undo statement. Remove collider.
                                                                Undo.DestroyObjectImmediate(aComp);
                                                                changedObjectsCount++;
                                                            }
                                                            else
                                                            {
                                                                Debug.Log("Unable to remove BoxCollider on object named : " + childTransform.name + " due to dependencies");
                                                            }
                                                        }
                                                    }
                                                }
                                                break;

                                            case "Capsule":
                                                {
                                                    var collidersToRemove = childTransform.gameObject.GetComponents(typeof(CapsuleCollider));
                                                    //find if we have one or multiple colliders to remove. If so, we remove them all if they match the type.
                                                    foreach (CapsuleCollider aComp in collidersToRemove)
                                                    {
                                                        if (aComp != null)
                                                        {
                                                            if (childTransform.gameObject.CanDestroy(aComp.GetType()))
                                                            {
                                                                //Record Undo statement. Remove collider.
                                                                Undo.DestroyObjectImmediate(aComp);
                                                                changedObjectsCount++;
                                                            }
                                                            else
                                                            {
                                                                Debug.Log("Unable to remove CapsuleCollider on object named : " + childTransform.name + " due to dependencies");
                                                            }
                                                        }
                                                    }
                                                }
                                                break;

                                            case "Mesh":
                                                {
                                                    var collidersToRemove = childTransform.gameObject.GetComponents(typeof(MeshCollider));
                                                    //find if we have one or multiple colliders to remove. If so, we remove them all if they match the type.
                                                    foreach (MeshCollider aComp in collidersToRemove)
                                                    {
                                                        if (aComp != null)
                                                        {
                                                            if (childTransform.gameObject.CanDestroy(aComp.GetType()))
                                                            {
                                                                //Record Undo statement. Remove collider.
                                                                Undo.DestroyObjectImmediate(aComp);
                                                                changedObjectsCount++;
                                                            }
                                                            else
                                                            {
                                                                Debug.Log("Unable to remove MeshCollider on object named : " + childTransform.name + " due to dependencies");
                                                            }
                                                        }
                                                    }
                                                }
                                                break;
                                        }

                                    }
                                    catch
                                    {
                                        Debug.LogError("Unable to remove " + ColliderType + " collider on object named : '" + childTransform.name + "'");
                                    }
                                }
                                catch
                                {
                                    Debug.LogError("Unable to remove " + ColliderType + " collider on object named : '" + childTransform.name + "'");
                                }
                            }
                        }
                        else
                        {
                            //No search
                            try
                            {
                                try
                                {
                                    switch (ColliderType)
                                    {
                                        case "Sphere":
                                            {
                                                var collidersToRemove = childTransform.gameObject.GetComponents(typeof(SphereCollider));
                                                //find if we have one or multiple colliders to remove. If so, we remove them all if they match the type.
                                                foreach (SphereCollider aComp in collidersToRemove)
                                                {
                                                    if (aComp != null)
                                                    {
                                                        if (childTransform.gameObject.CanDestroy(aComp.GetType()))
                                                        {
                                                            //Record Undo statement. Remove collider.
                                                            Undo.DestroyObjectImmediate(aComp);
                                                            changedObjectsCount++;
                                                        }
                                                        else
                                                        {
                                                            Debug.Log("Unable to remove SphereCollider on object named : " + childTransform.name + " due to dependencies");
                                                        }
                                                    }
                                                }
                                            }
                                            break;

                                        case "Box":
                                            {
                                                var collidersToRemove = childTransform.gameObject.GetComponents(typeof(BoxCollider));
                                                //find if we have one or multiple colliders to remove. If so, we remove them all if they match the type.
                                                foreach (BoxCollider aComp in collidersToRemove)
                                                {
                                                    if (aComp != null)
                                                    {
                                                        if (childTransform.gameObject.CanDestroy(aComp.GetType()))
                                                        {
                                                            //Record Undo statement. Remove collider.
                                                            Undo.DestroyObjectImmediate(aComp);
                                                            changedObjectsCount++;
                                                        }
                                                        else
                                                        {
                                                            Debug.Log("Unable to remove BoxCollider on object named : " + childTransform.name + " due to dependencies");
                                                        }
                                                    }
                                                }
                                            }
                                            break;

                                        case "Capsule":
                                            {
                                                var collidersToRemove = childTransform.gameObject.GetComponents(typeof(CapsuleCollider));
                                                //find if we have one or multiple colliders to remove. If so, we remove them all if they match the type.
                                                foreach (CapsuleCollider aComp in collidersToRemove)
                                                {
                                                    if (aComp != null)
                                                    {
                                                        if (childTransform.gameObject.CanDestroy(aComp.GetType()))
                                                        {
                                                            //Record Undo statement. Remove collider.
                                                            Undo.DestroyObjectImmediate(aComp);
                                                            changedObjectsCount++;
                                                        }
                                                        else
                                                        {
                                                            Debug.Log("Unable to remove CapsuleCollider on object named : " + childTransform.name + " due to dependencies");
                                                        }
                                                    }
                                                }
                                            }
                                            break;

                                        case "Mesh":
                                            {
                                                var collidersToRemove = childTransform.gameObject.GetComponents(typeof(MeshCollider));
                                                //find if we have one or multiple colliders to remove. If so, we remove them all if they match the type.
                                                foreach (MeshCollider aComp in collidersToRemove)
                                                {
                                                    if (aComp != null)
                                                    {
                                                        if (childTransform.gameObject.CanDestroy(aComp.GetType()))
                                                        {
                                                            //Record Undo statement. Remove collider.
                                                            Undo.DestroyObjectImmediate(aComp);
                                                            changedObjectsCount++;
                                                        }
                                                        else
                                                        {
                                                            Debug.Log("Unable to remove MeshCollider on object named : " + childTransform.name + " due to dependencies");
                                                        }
                                                    }
                                                }
                                            }
                                            break;
                                    }
                                }
                                catch
                                {
                                    Debug.LogError("Unable to remove " + ColliderType + " collider on object named : '" + childTransform.name + "'");
                                }
                            }
                            catch
                            {
                                Debug.LogError("Unable to remove " + ColliderType + " collider on object named : '" + childTransform.name + "'");
                            }
                        }
                    }
                }
            }
            finally
            {
                if (showProgressBar)
                {
                    EditorUtility.ClearProgressBar();
                }

                if (changedObjectsCount > 0)
                {
                    if (changedObjectsCount == 1)
                    {
                        Debug.LogFormat("1 " + ColliderType + " collider removed");
                        consoleOutput = "1 " + ColliderType + " collider removed";
                    }
                    else
                    {
                        Debug.LogFormat("{0} " + ColliderType + " colliders removed", changedObjectsCount);
                        consoleOutput = string.Format("{0} " + ColliderType + " colliders removed", changedObjectsCount);
                    }
                }
                else
                {
                    Debug.LogFormat("No " + ColliderType + " colliders removed");
                    consoleOutput = "No " + ColliderType + " colliders removed";
                }

            }
        }

        #endregion
         
        #region RendererControl

        /// <summary>
        /// These are the functions that perform the renderer tool actions.
        /// </summary>
        
        static void DisableRenderers()
        {
            GameObject[] objs = Selection.gameObjects;
            //Here we have the gameobjects selected.
            int numberOfTransforms = objs.Length;
            bool showProgressBar = numberOfTransforms >= SelectionThresholdForProgressBar;
            int changedObjectsCount = 0;

            try
            {
                for (int i = 0; i < numberOfTransforms; i++)
                {

                    if (showProgressBar)
                    {
                        EditorUtility.DisplayProgressBar("Disable renderers", "Disabling renderers (" + i + "/" + numberOfTransforms + " parent objects)",
                            (float)i / (float)numberOfTransforms);
                    }

                    Transform[] allTransforms;

                    //Must we disable child renderers also?
                    if (includeChildrenRenderers)
                    {
                        allTransforms = objs[i].GetComponentsInChildren<Transform>();
                    }
                    else
                    {
                        allTransforms = objs[i].GetComponents<Transform>();
                    }


                    foreach (Transform childTransform in allTransforms)
                    {
                        if (useSearch)
                        {
                            bool matchedSearch = false;

                            if (sType == SEARCHTYPE.NAME)
                            {
                                if (searchCaseSentive)
                                {
                                    switch (op)
                                    {
                                        case SEARCHOPTIONS.STARTS_WITH:
                                            if (childTransform.name.StartsWith(searchString))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;

                                        case SEARCHOPTIONS.CONTAINS:
                                            if (childTransform.name.Contains(searchString))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;

                                        case SEARCHOPTIONS.ENDS_WITH:
                                            if (childTransform.name.EndsWith(searchString))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;
                                    }
                                }
                                else
                                {
                                    switch (op)
                                    {
                                        case SEARCHOPTIONS.STARTS_WITH:
                                            if (childTransform.name.ToUpper().StartsWith(searchString.ToUpper()))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;

                                        case SEARCHOPTIONS.CONTAINS:
                                            if (childTransform.name.ToUpper().Contains(searchString.ToUpper()))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;

                                        case SEARCHOPTIONS.ENDS_WITH:
                                            if (childTransform.name.ToUpper().EndsWith(searchString.ToUpper()))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;
                                    }
                                }
                            }

                            if (sType == SEARCHTYPE.TAG)
                            {
                                if (searchTagCaseSentive)
                                {
                                    if (childTransform.tag.ToUpper().Equals(searchTag.ToUpper()))
                                    {
                                        matchedSearch = true;
                                    }
                                }
                                else
                                {
                                    if (childTransform.CompareTag(searchTag))
                                    {
                                        matchedSearch = true;
                                    }
                                }
                            }

                            if (sType == SEARCHTYPE.LAYER)
                            {
                                if (childTransform.gameObject.layer == searchLayer)
                                {
                                    matchedSearch = true;
                                }
                            }

                            //We need to only use go as the game object if it either matches the seach, or else do everything
                            if (matchedSearch == true)
                            {
                                try
                                {
                                    Renderer[] allRenderers;
                                    //find all renderers on the transform
                                    allRenderers = childTransform.GetComponents<Renderer>();
                                    
                                    if (allRenderers.Length > 0)
                                    {
                                        foreach (Renderer aChild in allRenderers)
                                        {
                                            //Record undo. Disable renderer
                                            Undo.RecordObject(aChild, "Disable renderer(s)");
                                            aChild.enabled = false;
                                            changedObjectsCount++;
                                        }
                                    }
                                }
                                catch
                                {
                                    Debug.LogError("Unable to get renderers on object named : '" + childTransform.name + "'");
                                }
                            }
                        }
                        else
                        {
                            //No search
                            try
                            {
                                Renderer[] allRenderers;
                                //find all renderers on the transform
                                allRenderers = childTransform.GetComponents<Renderer>();
                                
                                if (allRenderers.Length > 0)
                                {
                                    foreach (Renderer aChild in allRenderers)
                                    {
                                        //Record undo. Disable renderer
                                        Undo.RecordObject(aChild, "Disable renderer(s)");
                                        aChild.enabled = false;
                                        changedObjectsCount++;
                                    }
                                }
                            }
                            catch
                            {
                                Debug.LogError("Unable to get renderers on object named : '" + childTransform.name + "'");
                            }
                        }
                    }
                }
            }
            finally
            {
                if (showProgressBar)
                {
                    EditorUtility.ClearProgressBar();
                }

                if (changedObjectsCount > 0)
                {
                    if (changedObjectsCount == 1)
                    {
                        Debug.LogFormat("1 renderer disabled");
                        consoleOutput = "1 renderer disabled";
                    }
                    else
                    {
                        Debug.LogFormat("{0} renderers disabled", changedObjectsCount);
                        consoleOutput = string.Format("{0} renderers disabled", changedObjectsCount);
                    }
                }
                else
                {
                    Debug.LogFormat("No changes made");
                    consoleOutput = "No changes made";
                }
            }
        }

        static void EnableRenderers()
        {
            GameObject[] objs = Selection.gameObjects;
            //Here we have the gameobjects selected.
            int numberOfTransforms = objs.Length;
            bool showProgressBar = numberOfTransforms >= SelectionThresholdForProgressBar;
            int changedObjectsCount = 0;

            try
            {
                for (int i = 0; i < numberOfTransforms; i++)
                {

                    if (showProgressBar)
                    {
                        EditorUtility.DisplayProgressBar("Enable renderers", "Enabling renderers (" + i + "/" + numberOfTransforms + " parent objects)",
                            (float)i / (float)numberOfTransforms);
                    }

                    Transform[] allTransforms;

                    //are we enabling child renderers also?
                    if (includeChildrenRenderers)
                    {
                        allTransforms = objs[i].GetComponentsInChildren<Transform>();
                    }
                    else
                    {
                        allTransforms = objs[i].GetComponents<Transform>();
                    }


                    foreach (Transform childTransform in allTransforms)
                    {                        
                        if (useSearch)
                        {
                            bool matchedSearch = false;

                            if (sType == SEARCHTYPE.NAME)
                            {
                                if (searchCaseSentive)
                                {
                                    switch (op)
                                    {
                                        case SEARCHOPTIONS.STARTS_WITH:
                                            if (childTransform.name.StartsWith(searchString))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;

                                        case SEARCHOPTIONS.CONTAINS:
                                            if (childTransform.name.Contains(searchString))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;

                                        case SEARCHOPTIONS.ENDS_WITH:
                                            if (childTransform.name.EndsWith(searchString))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;
                                    }
                                }
                                else
                                {
                                    switch (op)
                                    {
                                        case SEARCHOPTIONS.STARTS_WITH:
                                            if (childTransform.name.ToUpper().StartsWith(searchString.ToUpper()))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;

                                        case SEARCHOPTIONS.CONTAINS:
                                            if (childTransform.name.ToUpper().Contains(searchString.ToUpper()))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;

                                        case SEARCHOPTIONS.ENDS_WITH:
                                            if (childTransform.name.ToUpper().EndsWith(searchString.ToUpper()))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;
                                    }
                                }
                            }

                            if (sType == SEARCHTYPE.TAG)
                            {
                                if (searchTagCaseSentive)
                                {
                                    if (childTransform.tag.ToUpper().Equals(searchTag.ToUpper()))
                                    {
                                        matchedSearch = true;
                                    }
                                }
                                else
                                {
                                    if (childTransform.CompareTag(searchTag))
                                    {
                                        matchedSearch = true;
                                    }
                                }
                            }

                            if (sType == SEARCHTYPE.LAYER)
                            {
                                if (childTransform.gameObject.layer == searchLayer)
                                {
                                    matchedSearch = true;
                                }
                            }

                            //We need to only use go as the game object if it either matches the seach, or else do everything
                            if (matchedSearch == true)
                            {
                                try
                                {
                                    Renderer[] allRenderers;
                                    //find all of the renderers on the current transform
                                    allRenderers = childTransform.GetComponents<Renderer>();

                                    //we now have all the child renderers
                                    if (allRenderers.Length > 0)
                                    {
                                        foreach (Renderer aChild in allRenderers)
                                        {
                                            //Record undo. Enable renderer
                                            Undo.RecordObject(aChild, "Enable renderer(s)");
                                            aChild.enabled = true;
                                            changedObjectsCount++;
                                        }
                                    }
                                }
                                catch
                                {
                                    Debug.LogError("Unable to get renderers on object named : '" + childTransform.name + "'");
                                }
                            }
                        }
                        else
                        {
                           //No search
                            try
                            {
                                Renderer[] allRenderers;
                                //get all renderers on current transform
                                allRenderers = childTransform.GetComponents<Renderer>();

                                //we now have all the child renderers
                                if (allRenderers.Length > 0)
                                {
                                    foreach (Renderer aChild in allRenderers)
                                    {
                                        //Record undo. Enable renderer
                                        Undo.RecordObject(aChild, "Enable renderer(s)");
                                        aChild.enabled = true;
                                        changedObjectsCount++;
                                    }
                                }
                            }
                            catch
                            {
                                Debug.LogError("Unable to get renderers on object named : '" + childTransform.name + "'");
                            }
                        }
                    }
                }
            }
            finally
            {
                if (showProgressBar)
                {
                    EditorUtility.ClearProgressBar();
                }

                if (changedObjectsCount > 0)
                {
                    if (changedObjectsCount == 1)
                    {
                        Debug.LogFormat("1 renderer enabled");
                        consoleOutput = "1 renderer enabled";
                    }
                    else
                    {
                        Debug.LogFormat("{0} renderers enabled", changedObjectsCount);
                        consoleOutput = string.Format("{0} renderers enabled", changedObjectsCount);
                    }
                }
                else
                {
                    Debug.LogFormat("No changes made");
                    consoleOutput = "No changes made";
                }
            }
        }

        static void DisableCastShadows()
        {
            GameObject[] objs = Selection.gameObjects;
            //Here we have the gameobjects selected.
            int numberOfTransforms = objs.Length;
            bool showProgressBar = numberOfTransforms >= SelectionThresholdForProgressBar;
            int changedObjectsCount = 0;

            try
            {
                for (int i = 0; i < numberOfTransforms; i++)
                {

                    if (showProgressBar)
                    {
                        EditorUtility.DisplayProgressBar("Disable shadow casting", "Disabling shadow casting (" + i + "/" + numberOfTransforms + " parent objects)",
                            (float)i / (float)numberOfTransforms);
                    }


                    Transform[] allTransforms;

                    //Must we turn shadows off on all child objects
                    if (includeChildrenRenderers)
                    {
                        allTransforms = objs[i].GetComponentsInChildren<Transform>();
                    }
                    else
                    {
                        allTransforms = objs[i].GetComponents<Transform>();
                    }


                    foreach (Transform childTransform in allTransforms)
                    {                        
                        if (useSearch)
                        {
                            bool matchedSearch = false;

                            if (sType == SEARCHTYPE.NAME)
                            {
                                if (searchCaseSentive)
                                {
                                    switch (op)
                                    {
                                        case SEARCHOPTIONS.STARTS_WITH:
                                            if (childTransform.name.StartsWith(searchString))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;

                                        case SEARCHOPTIONS.CONTAINS:
                                            if (childTransform.name.Contains(searchString))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;

                                        case SEARCHOPTIONS.ENDS_WITH:
                                            if (childTransform.name.EndsWith(searchString))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;
                                    }
                                }
                                else
                                {
                                    switch (op)
                                    {
                                        case SEARCHOPTIONS.STARTS_WITH:
                                            if (childTransform.name.ToUpper().StartsWith(searchString.ToUpper()))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;

                                        case SEARCHOPTIONS.CONTAINS:
                                            if (childTransform.name.ToUpper().Contains(searchString.ToUpper()))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;

                                        case SEARCHOPTIONS.ENDS_WITH:
                                            if (childTransform.name.ToUpper().EndsWith(searchString.ToUpper()))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;
                                    }
                                }
                            }

                            if (sType == SEARCHTYPE.TAG)
                            {
                                if (searchTagCaseSentive)
                                {
                                    if (childTransform.tag.ToUpper().Equals(searchTag.ToUpper()))
                                    {
                                        matchedSearch = true;
                                    }
                                }
                                else
                                {
                                    if (childTransform.CompareTag(searchTag))
                                    {
                                        matchedSearch = true;
                                    }
                                }
                            }

                            if (sType == SEARCHTYPE.LAYER)
                            {
                                if (childTransform.gameObject.layer == searchLayer)
                                {
                                    matchedSearch = true;
                                }
                            }

                            //We need to only use go as the game object if it either matches the seach, or else do everything
                            if (matchedSearch == true)
                            {
                                try
                                {
                                    Renderer[] allRenderers;
                                    //get all renderers on current transform
                                    allRenderers = childTransform.GetComponents<Renderer>();

                                    //we now have all the child renderers
                                    if (allRenderers.Length > 0)
                                    {
                                        foreach (Renderer aChild in allRenderers)
                                        {
                                            //Record undo. Disable shadow casting
                                            Undo.RecordObject(aChild, "Disable Shadow Casting");
                                            aChild.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                                            changedObjectsCount++;
                                        }
                                    }
                                }
                                catch
                                {
                                    Debug.LogError("Unable to get renderer shadow casting on object named : '" + childTransform.name + "'");
                                }
                            }
                        }
                        else
                        {
                            //No search
                            try
                            {
                                Renderer[] allRenderers;
                                allRenderers = childTransform.GetComponents<Renderer>();


                                //we now have all the child renderers
                                if (allRenderers.Length > 0)
                                {
                                    foreach (Renderer aChild in allRenderers)
                                    {
                                        //Record undo. Disable shadow casting
                                        Undo.RecordObject(aChild, "Disable Shadow Casting");
                                        aChild.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                                        changedObjectsCount++;
                                    }
                                }
                            }
                            catch
                            {
                                Debug.LogError("Unable to get renderer shadow casting on object named : '" + childTransform.name + "'");
                            }
                        }
                    }
                }
            }
            finally
            {
                if (showProgressBar)
                {
                    EditorUtility.ClearProgressBar();
                }

                if (changedObjectsCount > 0)
                {
                    if (changedObjectsCount == 1)
                    {
                        Debug.LogFormat("1 shadow caster turned off");
                        consoleOutput = "1 shadow caster turned off";
                    }
                    else
                    {
                        Debug.LogFormat("{0} shadow casters turned off", changedObjectsCount);
                        consoleOutput = string.Format("{0} shadow casters turned off", changedObjectsCount);
                    }
                }
                else
                {
                    Debug.LogFormat("No changes made");
                    consoleOutput = "No changes made";
                }
            }
        }

        static void EnableCastShadows()
        {
            GameObject[] objs = Selection.gameObjects;
            //Here we have the gameobjects selected.
            int numberOfTransforms = objs.Length;
            bool showProgressBar = numberOfTransforms >= SelectionThresholdForProgressBar;
            int changedObjectsCount = 0;

            try
            {
                for (int i = 0; i < numberOfTransforms; i++)
                {

                    if (showProgressBar)
                    {
                        EditorUtility.DisplayProgressBar("Enable shadow casting", "Enabling shadow casting (" + i + "/" + numberOfTransforms + " parent objects)",
                            (float)i / (float)numberOfTransforms);
                    }

                    Transform[] allTransforms;

                    //do we enable shadow casting on child objects also?
                    if (includeChildrenRenderers)
                    {
                        allTransforms = objs[i].GetComponentsInChildren<Transform>();
                    }
                    else
                    {
                        allTransforms = objs[i].GetComponents<Transform>();
                    }


                    foreach (Transform childTransform in allTransforms)
                    {
                        if (useSearch)
                        {
                            bool matchedSearch = false;

                            if (sType == SEARCHTYPE.NAME)
                            {
                                if (searchCaseSentive)
                                {
                                    switch (op)
                                    {
                                        case SEARCHOPTIONS.STARTS_WITH:
                                            if (childTransform.name.StartsWith(searchString))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;

                                        case SEARCHOPTIONS.CONTAINS:
                                            if (childTransform.name.Contains(searchString))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;

                                        case SEARCHOPTIONS.ENDS_WITH:
                                            if (childTransform.name.EndsWith(searchString))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;
                                    }
                                }
                                else
                                {
                                    switch (op)
                                    {
                                        case SEARCHOPTIONS.STARTS_WITH:
                                            if (childTransform.name.ToUpper().StartsWith(searchString.ToUpper()))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;

                                        case SEARCHOPTIONS.CONTAINS:
                                            if (childTransform.name.ToUpper().Contains(searchString.ToUpper()))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;

                                        case SEARCHOPTIONS.ENDS_WITH:
                                            if (childTransform.name.ToUpper().EndsWith(searchString.ToUpper()))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;
                                    }
                                }
                            }

                            if (sType == SEARCHTYPE.TAG)
                            {
                                if (searchTagCaseSentive)
                                {
                                    if (childTransform.tag.ToUpper().Equals(searchTag.ToUpper()))
                                    {
                                        matchedSearch = true;
                                    }
                                }
                                else
                                {
                                    if (childTransform.CompareTag(searchTag))
                                    {
                                        matchedSearch = true;
                                    }
                                }
                            }

                            if (sType == SEARCHTYPE.LAYER)
                            {
                                if (childTransform.gameObject.layer == searchLayer)
                                {
                                    matchedSearch = true;
                                }
                            }

                            //We need to only use go as the game object if it either matches the seach, or else do everything
                            if (matchedSearch == true)
                            {
                                try
                                {
                                    Renderer[] allRenderers;
                                    allRenderers = childTransform.GetComponents<Renderer>();

                                    //we now have all the child renderers
                                    if (allRenderers.Length > 0)
                                    {
                                        foreach (Renderer aChild in allRenderers)
                                        {
                                            //Record undo. Enable shadow casting
                                            Undo.RecordObject(aChild, "Enable Shadow Casting");
                                            aChild.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                                            changedObjectsCount++;
                                        }
                                    }
                                }
                                catch
                                {
                                    Debug.LogError("Unable to get renderer shadow casting on object named : '" + childTransform.name + "'");
                                }
                            }
                        }
                        else
                        {
                            //No search
                            try
                            {
                                Renderer[] allRenderers;
                                allRenderers = childTransform.GetComponents<Renderer>();

                                //we now have all the child colliders
                                if (allRenderers.Length > 0)
                                {
                                    foreach (Renderer aChild in allRenderers)
                                    {
                                        //Record undo. Enable shadow casting
                                        Undo.RecordObject(aChild, "Enable Shadow Casting");
                                        aChild.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                                        changedObjectsCount++;
                                    }
                                }
                            }
                            catch
                            {
                                Debug.LogError("Unable to get renderer shadow casting on object named : '" + childTransform.name + "'");
                            }
                        }
                    }
                }
            }
            finally
            {
                if (showProgressBar)
                {
                    EditorUtility.ClearProgressBar();
                }

                if (changedObjectsCount > 0)
                {
                    if (changedObjectsCount == 1)
                    {
                        Debug.LogFormat("1 shadow caster turned on");
                        consoleOutput = "1 shadow caster turned on";
                    }
                    else
                    {
                        Debug.LogFormat("{0} shadow casters turned on", changedObjectsCount);
                        consoleOutput = string.Format("{0} shadow casters turned on", changedObjectsCount);
                    }
                }
                else
                {
                    Debug.LogFormat("No changes made");
                    consoleOutput = "No changes made";
                }

            }
        }

        static void EnableCastShadowsTwoSided()
        {
            GameObject[] objs = Selection.gameObjects;
            //Here we have the gameobjects selected.
            int numberOfTransforms = objs.Length;
            bool showProgressBar = numberOfTransforms >= SelectionThresholdForProgressBar;
            int changedObjectsCount = 0;

            try
            {
                for (int i = 0; i < numberOfTransforms; i++)
                {

                    if (showProgressBar)
                    {
                        EditorUtility.DisplayProgressBar("Set shadow casting to Two Sided", "Setting shadow casting to Two Sided (" + i + "/" + numberOfTransforms + " parent objects)",
                            (float)i / (float)numberOfTransforms);
                    }

                    Transform[] allTransforms;

                    //Do we enable two-sided shadow casting on child objects also?
                    if (includeChildrenRenderers)
                    {
                        allTransforms = objs[i].GetComponentsInChildren<Transform>();
                    }
                    else
                    {
                        allTransforms = objs[i].GetComponents<Transform>();
                    }


                    foreach (Transform childTransform in allTransforms)
                    {
                        if (useSearch)
                        {
                            bool matchedSearch = false;

                            if (sType == SEARCHTYPE.NAME)
                            {
                                if (searchCaseSentive)
                                {
                                    switch (op)
                                    {
                                        case SEARCHOPTIONS.STARTS_WITH:
                                            if (childTransform.name.StartsWith(searchString))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;

                                        case SEARCHOPTIONS.CONTAINS:
                                            if (childTransform.name.Contains(searchString))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;

                                        case SEARCHOPTIONS.ENDS_WITH:
                                            if (childTransform.name.EndsWith(searchString))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;
                                    }
                                }
                                else
                                {
                                    switch (op)
                                    {
                                        case SEARCHOPTIONS.STARTS_WITH:
                                            if (childTransform.name.ToUpper().StartsWith(searchString.ToUpper()))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;

                                        case SEARCHOPTIONS.CONTAINS:
                                            if (childTransform.name.ToUpper().Contains(searchString.ToUpper()))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;

                                        case SEARCHOPTIONS.ENDS_WITH:
                                            if (childTransform.name.ToUpper().EndsWith(searchString.ToUpper()))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;
                                    }
                                }
                            }

                            if (sType == SEARCHTYPE.TAG)
                            {
                                if (searchTagCaseSentive)
                                {
                                    if (childTransform.tag.ToUpper().Equals(searchTag.ToUpper()))
                                    {
                                        matchedSearch = true;
                                    }
                                }
                                else
                                {
                                    if (childTransform.CompareTag(searchTag))
                                    {
                                        matchedSearch = true;
                                    }
                                }
                            }

                            if (sType == SEARCHTYPE.LAYER)
                            {
                                if (childTransform.gameObject.layer == searchLayer)
                                {
                                    matchedSearch = true;
                                }
                            }

                            //We need to only use go as the game object if it either matches the seach, or else do everything
                            if (matchedSearch == true)
                            {
                                try
                                {
                                    Renderer[] allRenderers;
                                    allRenderers = childTransform.GetComponents<Renderer>();

                                    //we now have all the child renderers
                                    if (allRenderers.Length > 0)
                                    {
                                        foreach (Renderer aChild in allRenderers)
                                        {
                                            //Record undo. Set Two Sided shadow casting
                                            Undo.RecordObject(aChild, "Shadow Casting Two Sided");
                                            aChild.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.TwoSided;
                                            changedObjectsCount++;
                                        }
                                    }
                                }
                                catch
                                {
                                    Debug.LogError("Unable to get renderer shadow casting on object named : '" + childTransform.name + "'");
                                }
                            }
                        }
                        else
                        {
                            //No search
                            try
                            {
                                Renderer[] allRenderers;
                                allRenderers = childTransform.GetComponents<Renderer>();

                                //we now have all the child renderers
                                if (allRenderers.Length > 0)
                                {
                                    foreach (Renderer aChild in allRenderers)
                                    {
                                        //Record undo. Set Two Sided shadow casting
                                        Undo.RecordObject(aChild, "Shadow Casting Two Sided");
                                        aChild.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.TwoSided;
                                        changedObjectsCount++;
                                    }
                                }
                            }
                            catch
                            {
                                Debug.LogError("Unable to get renderer shadow casting on object named : '" + childTransform.name + "'");
                            }
                        }
                    }
                }
            }
            finally
            {
                if (showProgressBar)
                {
                    EditorUtility.ClearProgressBar();
                }

                if (changedObjectsCount > 0)
                {
                    if (changedObjectsCount == 1)
                    {
                        Debug.LogFormat("1 shadow caster set to Two Sided");
                        consoleOutput = "1 shadow caster set to Two Sided";
                    }
                    else
                    {
                        Debug.LogFormat("{0} shadow casters set to Two Sided", changedObjectsCount);
                        consoleOutput = string.Format("{0} shadow casters set to Two Sided", changedObjectsCount);
                    }
                }
                else
                {
                    Debug.LogFormat("No changes made");
                    consoleOutput = "No changes made";
                }

            }
        }

        static void EnableCastShadowsOnly()
        {
            GameObject[] objs = Selection.gameObjects;
            //Here we have the gameobjects selected.
            int numberOfTransforms = objs.Length;
            bool showProgressBar = numberOfTransforms >= SelectionThresholdForProgressBar;
            int changedObjectsCount = 0;

            try
            {
                for (int i = 0; i < numberOfTransforms; i++)
                {

                    if (showProgressBar)
                    {
                        EditorUtility.DisplayProgressBar("Set shadow casting to Shadows Only", "Setting shadow casting to Shadows Only (" + i + "/" + numberOfTransforms + " parent objects)",
                            (float)i / (float)numberOfTransforms);
                    }

                    Transform[] allTransforms;

                    //Do we want to set child objects to shadow casting only?
                    if (includeChildrenRenderers)
                    {
                        allTransforms = objs[i].GetComponentsInChildren<Transform>();
                    }
                    else
                    {
                        allTransforms = objs[i].GetComponents<Transform>();
                    }


                    foreach (Transform childTransform in allTransforms)
                    {                        
                        if (useSearch)
                        {
                            bool matchedSearch = false;

                            if (sType == SEARCHTYPE.NAME)
                            {
                                if (searchCaseSentive)
                                {
                                    switch (op)
                                    {
                                        case SEARCHOPTIONS.STARTS_WITH:
                                            if (childTransform.name.StartsWith(searchString))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;

                                        case SEARCHOPTIONS.CONTAINS:
                                            if (childTransform.name.Contains(searchString))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;

                                        case SEARCHOPTIONS.ENDS_WITH:
                                            if (childTransform.name.EndsWith(searchString))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;
                                    }
                                }
                                else
                                {
                                    switch (op)
                                    {
                                        case SEARCHOPTIONS.STARTS_WITH:
                                            if (childTransform.name.ToUpper().StartsWith(searchString.ToUpper()))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;

                                        case SEARCHOPTIONS.CONTAINS:
                                            if (childTransform.name.ToUpper().Contains(searchString.ToUpper()))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;

                                        case SEARCHOPTIONS.ENDS_WITH:
                                            if (childTransform.name.ToUpper().EndsWith(searchString.ToUpper()))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;
                                    }
                                }
                            }

                            if (sType == SEARCHTYPE.TAG)
                            {
                                if (searchTagCaseSentive)
                                {
                                    if (childTransform.tag.ToUpper().Equals(searchTag.ToUpper()))
                                    {
                                        matchedSearch = true;
                                    }
                                }
                                else
                                {
                                    if (childTransform.CompareTag(searchTag))
                                    {
                                        matchedSearch = true;
                                    }
                                }
                            }

                            if (sType == SEARCHTYPE.LAYER)
                            {
                                if (childTransform.gameObject.layer == searchLayer)
                                {
                                    matchedSearch = true;
                                }
                            }

                            //We need to only use go as the game object if it either matches the seach, or else do everything
                            if (matchedSearch == true)
                            {
                                try
                                {
                                    Renderer[] allRenderers;
                                    allRenderers = childTransform.GetComponents<Renderer>();

                                    //we now have all the child renderers
                                    if (allRenderers.Length > 0)
                                    {
                                        foreach (Renderer aChild in allRenderers)
                                        {
                                            //Record undo. Set shadow casting only
                                            Undo.RecordObject(aChild, "Shadow Casting Shadows Only");
                                            aChild.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
                                            changedObjectsCount++;
                                        }
                                    }
                                }
                                catch
                                {
                                    Debug.LogError("Unable to get renderer shadow casting on object named : '" + childTransform.name + "'");
                                }
                            }
                        }
                        else
                        {
                            //No search
                            try
                            {
                                Renderer[] allRenderers;
                                allRenderers = childTransform.GetComponents<Renderer>();

                                //we now have all the child renderers
                                if (allRenderers.Length > 0)
                                {
                                    foreach (Renderer aChild in allRenderers)
                                    {
                                        //Record undo. Set shadow casting only
                                        Undo.RecordObject(aChild, "Shadow Casting Shadows Only");
                                        aChild.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
                                        changedObjectsCount++;
                                    }
                                }
                            }
                            catch
                            {
                                Debug.LogError("Unable to get renderer shadow casting on object named : '" + childTransform.name + "'");
                            }
                        }
                    }
                }
            }
            finally
            {
                if (showProgressBar)
                {
                    EditorUtility.ClearProgressBar();
                }

                if (changedObjectsCount > 0)
                {
                    if (changedObjectsCount == 1)
                    {
                        Debug.LogFormat("1 shadow caster turned set to Shadows Only");
                        consoleOutput = "1 shadow caster turned set to Shadows Only";
                    }
                    else
                    {
                        Debug.LogFormat("{0} shadow casters set to Shadows Only", changedObjectsCount);
                        consoleOutput = string.Format("{0} shadow casters set to Shadows Only", changedObjectsCount);
                    }
                }
                else
                {
                    Debug.LogFormat("No changes made");
                    consoleOutput = "No changes made";
                }

            }
        }

        #endregion

        #region Rigidbodycontrol

        /// <summary>
        /// These are the functions that perform the renderer tool actions.
        /// </summary>

        static void DisableIsKinematic()
        {
            GameObject[] objs = Selection.gameObjects;
            //Here we have the gameobjects selected.
            int numberOfTransforms = objs.Length;
            bool showProgressBar = numberOfTransforms >= SelectionThresholdForProgressBar;
            int changedObjectsCount = 0;

            try
            {
                for (int i = 0; i < numberOfTransforms; i++)
                {

                    if (showProgressBar)
                    {
                        EditorUtility.DisplayProgressBar("Disable isKinematic", "Disabling isKinematic (" + i + "/" + numberOfTransforms + " parent objects)",
                            (float)i / (float)numberOfTransforms);
                    }

                    Transform[] allTransforms;

                    //Do we want to disable isKinematic on child objects also?
                    if (includeChildrenRigidbodies)
                    {
                        allTransforms = objs[i].GetComponentsInChildren<Transform>();
                    }
                    else
                    {
                        allTransforms = objs[i].GetComponents<Transform>();
                    }


                    foreach (Transform childTransform in allTransforms)
                    {

                        if (useSearch)
                        {
                            bool matchedSearch = false;

                            if (sType == SEARCHTYPE.NAME)
                            {
                                if (searchCaseSentive)
                                {
                                    switch (op)
                                    {
                                        case SEARCHOPTIONS.STARTS_WITH:
                                            if (childTransform.name.StartsWith(searchString))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;

                                        case SEARCHOPTIONS.CONTAINS:
                                            if (childTransform.name.Contains(searchString))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;

                                        case SEARCHOPTIONS.ENDS_WITH:
                                            if (childTransform.name.EndsWith(searchString))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;
                                    }
                                }
                                else
                                {
                                    switch (op)
                                    {
                                        case SEARCHOPTIONS.STARTS_WITH:
                                            if (childTransform.name.ToUpper().StartsWith(searchString.ToUpper()))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;

                                        case SEARCHOPTIONS.CONTAINS:
                                            if (childTransform.name.ToUpper().Contains(searchString.ToUpper()))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;

                                        case SEARCHOPTIONS.ENDS_WITH:
                                            if (childTransform.name.ToUpper().EndsWith(searchString.ToUpper()))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;
                                    }
                                }
                            }

                            if (sType == SEARCHTYPE.TAG)
                            {
                                if (searchTagCaseSentive)
                                {
                                    if (childTransform.tag.ToUpper().Equals(searchTag.ToUpper()))
                                    {
                                        matchedSearch = true;
                                    }
                                }
                                else
                                {
                                    if (childTransform.CompareTag(searchTag))
                                    {
                                        matchedSearch = true;
                                    }
                                }
                            }

                            if (sType == SEARCHTYPE.LAYER)
                            {
                                if (childTransform.gameObject.layer == searchLayer)
                                {
                                    matchedSearch = true;
                                }
                            }

                            //We need to only use go as the game object if it either matches the seach, or else do everything
                            if (matchedSearch == true)
                            {
                                try
                                {
                                    Rigidbody[] allBodies;
                                    allBodies = childTransform.GetComponents<Rigidbody>();

                                    //we now have all the child rigidbodies
                                    if (allBodies.Length > 0)
                                    {
                                        foreach (Rigidbody aChild in allBodies)
                                        {
                                            //Record undo. Disable isKinematic.
                                            Undo.RecordObject(aChild, "Disable isKinematic");
                                            aChild.isKinematic = false;
                                            changedObjectsCount++;
                                        }
                                    }
                                }
                                catch
                                {
                                    Debug.LogError("Unable to get rigidbodies on object named : '" + childTransform.name + "'");
                                }
                            }
                        }
                        else
                        {
                           //No search
                            try
                            {
                                Rigidbody[] allBodies;
                                allBodies = childTransform.GetComponents<Rigidbody>();

                                if (allBodies.Length > 0)
                                {
                                    foreach (Rigidbody aChild in allBodies)
                                    {
                                        //Record undo. Disable isKinematic.
                                        Undo.RecordObject(aChild, "Disable isKinematic");
                                        aChild.isKinematic = false;
                                        changedObjectsCount++;
                                    }
                                }
                            }
                            catch
                            {
                                Debug.LogError("Unable to get rigidbodies on object named : '" + childTransform.name + "'");
                            }
                        }
                    }
                }
            }
            finally
            {
                if (showProgressBar)
                {
                    EditorUtility.ClearProgressBar();
                }

                if (changedObjectsCount > 0)
                {
                    if (changedObjectsCount == 1)
                    {
                        Debug.LogFormat("isKinematic disabled on 1 rigidbody");
                        consoleOutput = "isKinematic disabled on 1 rigidbody";
                    }
                    else
                    {
                        Debug.LogFormat("isKinematic disabled on {0} rigidbodies", changedObjectsCount);
                        consoleOutput = string.Format("isKinematic disabled on {0} rigidbodies", changedObjectsCount);
                    }
                }
                else
                {
                    Debug.LogFormat("No changes made");
                    consoleOutput = "No changes made";
                }
            }
        }

        static void EnableIsKinematic()
        {
            GameObject[] objs = Selection.gameObjects;
            //Here we have the gameobjects selected.
            int numberOfTransforms = objs.Length;
            bool showProgressBar = numberOfTransforms >= SelectionThresholdForProgressBar;
            int changedObjectsCount = 0;

            try
            {
                for (int i = 0; i < numberOfTransforms; i++)
                {

                    if (showProgressBar)
                    {
                        EditorUtility.DisplayProgressBar("Enable isKinematic", "Enabling isKinematic (" + i + "/" + numberOfTransforms + " parent objects)",
                            (float)i / (float)numberOfTransforms);
                    }

                    Transform[] allTransforms;

                    //Do we want to enable isKinematic on child objects also?
                    if (includeChildrenRigidbodies)
                    {
                        allTransforms = objs[i].GetComponentsInChildren<Transform>();
                    }
                    else
                    {
                        allTransforms = objs[i].GetComponents<Transform>();
                    }

                    foreach (Transform childTransform in allTransforms)
                    {                        
                        if (useSearch)
                        {
                            bool matchedSearch = false;

                            if (sType == SEARCHTYPE.NAME)
                            {
                                if (searchCaseSentive)
                                {
                                    switch (op)
                                    {
                                        case SEARCHOPTIONS.STARTS_WITH:
                                            if (childTransform.name.StartsWith(searchString))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;

                                        case SEARCHOPTIONS.CONTAINS:
                                            if (childTransform.name.Contains(searchString))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;

                                        case SEARCHOPTIONS.ENDS_WITH:
                                            if (childTransform.name.EndsWith(searchString))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;
                                    }
                                }
                                else
                                {
                                    switch (op)
                                    {
                                        case SEARCHOPTIONS.STARTS_WITH:
                                            if (childTransform.name.ToUpper().StartsWith(searchString.ToUpper()))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;

                                        case SEARCHOPTIONS.CONTAINS:
                                            if (childTransform.name.ToUpper().Contains(searchString.ToUpper()))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;

                                        case SEARCHOPTIONS.ENDS_WITH:
                                            if (childTransform.name.ToUpper().EndsWith(searchString.ToUpper()))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;
                                    }
                                }
                            }

                            if (sType == SEARCHTYPE.TAG)
                            {
                                if (searchTagCaseSentive)
                                {
                                    if (childTransform.tag.ToUpper().Equals(searchTag.ToUpper()))
                                    {
                                        matchedSearch = true;
                                    }
                                }
                                else
                                {
                                    if (childTransform.CompareTag(searchTag))
                                    {
                                        matchedSearch = true;
                                    }
                                }
                            }

                            if (sType == SEARCHTYPE.LAYER)
                            {
                                if (childTransform.gameObject.layer == searchLayer)
                                {
                                    matchedSearch = true;
                                }
                            }

                            //We need to only use go as the game object if it either matches the seach, or else do everything
                            if (matchedSearch == true)
                            {
                                try
                                {
                                    Rigidbody[] allBodies;
                                    allBodies = childTransform.GetComponents<Rigidbody>();

                                    //we now have all the child rigidbodies
                                    if (allBodies.Length > 0)
                                    {
                                        foreach (Rigidbody aChild in allBodies)
                                        {
                                            //Record undo. Enable isKinematic.
                                            Undo.RecordObject(aChild, "Enable isKinematic");
                                            aChild.isKinematic = true;
                                            changedObjectsCount++;
                                        }
                                    }
                                }
                                catch
                                {
                                    Debug.LogError("Unable to get rigidbodies on object named : '" + childTransform.name + "'");
                                }
                            }
                        }
                        else
                        {
                            //No search
                            try
                            {
                                Rigidbody[] allBodies;
                                allBodies = childTransform.GetComponents<Rigidbody>();

                                //we now have all the child rigidbodies
                                if (allBodies.Length > 0)
                                {
                                    foreach (Rigidbody aChild in allBodies)
                                    {
                                        //Record undo. Enable isKinematic.
                                        Undo.RecordObject(aChild, "Enable isKinematic");
                                        aChild.isKinematic = true;
                                        changedObjectsCount++;
                                    }
                                }
                            }
                            catch
                            {
                                Debug.LogError("Unable to get rigidbodies on object named : '" + childTransform.name + "'");
                            }
                        }
                    }
                }
            }
            finally
            {
                if (showProgressBar)
                {
                    EditorUtility.ClearProgressBar();
                }

                if (changedObjectsCount > 0)
                {
                    if (changedObjectsCount == 1)
                    {
                        Debug.LogFormat("isKinematic enabled on 1 rigidbody");
                        consoleOutput = "isKinematic enabled on 1 rigidbody";
                    }
                    else
                    {
                        Debug.LogFormat("isKinematic enabled on {0} rigidbodies", changedObjectsCount);
                        consoleOutput = string.Format("isKinematic enabled on {0} rigidbodies", changedObjectsCount);
                    }
                }
                else
                {
                    Debug.LogFormat("No changes made");
                    consoleOutput = "No changes made";
                }
            }
        }

        static void DisableUseGravity()
        {
            GameObject[] objs = Selection.gameObjects;
            //Here we have the gameobjects selected.
            int numberOfTransforms = objs.Length;
            bool showProgressBar = numberOfTransforms >= SelectionThresholdForProgressBar;
            int changedObjectsCount = 0;

            try
            {
                for (int i = 0; i < numberOfTransforms; i++)
                {

                    if (showProgressBar)
                    {
                        EditorUtility.DisplayProgressBar("Disable Use Gravity", "Disabling Use Gravity (" + i + "/" + numberOfTransforms + " parent objects)",
                            (float)i / (float)numberOfTransforms);
                    }


                    Transform[] allTransforms;

                    //Do we want to disable Use Gravity on child objects also?
                    if (includeChildrenRigidbodies)
                    {
                        allTransforms = objs[i].GetComponentsInChildren<Transform>();
                    }
                    else
                    {
                        allTransforms = objs[i].GetComponents<Transform>();
                    }

                    foreach (Transform childTransform in allTransforms)
                    {                        
                        if (useSearch)
                        {
                            bool matchedSearch = false;

                            if (sType == SEARCHTYPE.NAME)
                            {
                                if (searchCaseSentive)
                                {
                                    switch (op)
                                    {
                                        case SEARCHOPTIONS.STARTS_WITH:
                                            if (childTransform.name.StartsWith(searchString))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;

                                        case SEARCHOPTIONS.CONTAINS:
                                            if (childTransform.name.Contains(searchString))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;

                                        case SEARCHOPTIONS.ENDS_WITH:
                                            if (childTransform.name.EndsWith(searchString))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;
                                    }
                                }
                                else
                                {
                                    switch (op)
                                    {
                                        case SEARCHOPTIONS.STARTS_WITH:
                                            if (childTransform.name.ToUpper().StartsWith(searchString.ToUpper()))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;

                                        case SEARCHOPTIONS.CONTAINS:
                                            if (childTransform.name.ToUpper().Contains(searchString.ToUpper()))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;

                                        case SEARCHOPTIONS.ENDS_WITH:
                                            if (childTransform.name.ToUpper().EndsWith(searchString.ToUpper()))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;
                                    }
                                }
                            }

                            if (sType == SEARCHTYPE.TAG)
                            {
                                if (searchTagCaseSentive)
                                {
                                    if (childTransform.tag.ToUpper().Equals(searchTag.ToUpper()))
                                    {
                                        matchedSearch = true;
                                    }
                                }
                                else
                                {
                                    if (childTransform.CompareTag(searchTag))
                                    {
                                        matchedSearch = true;
                                    }
                                }
                            }

                            if (sType == SEARCHTYPE.LAYER)
                            {
                                if (childTransform.gameObject.layer == searchLayer)
                                {
                                    matchedSearch = true;
                                }
                            }

                            //We need to only use go as the game object if it either matches the seach, or else do everything
                            if (matchedSearch == true)
                            {
                                try
                                {
                                    Rigidbody[] allBodies;
                                    allBodies = childTransform.GetComponents<Rigidbody>();

                                    //we now have all the child rigidbodies
                                    if (allBodies.Length > 0)
                                    {
                                        foreach (Rigidbody aChild in allBodies)
                                        {
                                            //Record undo. Disable Use Gravity.
                                            Undo.RecordObject(aChild, "Disable Use Gravity");
                                            aChild.useGravity = false;
                                            changedObjectsCount++;
                                        }
                                    }
                                }
                                catch
                                {
                                    Debug.LogError("Unable to get rigidbodies on object named : '" + childTransform.name + "'");
                                }
                            }
                        }
                        else
                        {
                            //No search
                            try
                            {
                                Rigidbody[] allBodies;
                                allBodies = childTransform.GetComponents<Rigidbody>();

                                //we now have all the child rigidbodies
                                if (allBodies.Length > 0)
                                {
                                    foreach (Rigidbody aChild in allBodies)
                                    {
                                        //Record undo. Disable Use Gravity.
                                        Undo.RecordObject(aChild, "Disable Use Gravity");
                                        aChild.useGravity = false;
                                        changedObjectsCount++;
                                    }
                                }
                            }
                            catch
                            {
                                Debug.LogError("Unable to get rigidbodies on object named : '" + childTransform.name + "'");
                            }
                        }
                    }
                }
            }
            finally
            {
                if (showProgressBar)
                {
                    EditorUtility.ClearProgressBar();
                }

                if (changedObjectsCount > 0)
                {
                    if (changedObjectsCount == 1)
                    {
                        Debug.LogFormat("Use Gravity disabled on 1 rigidbody");
                        consoleOutput = "Use Gravity disabled on 1 rigidbody";
                    }
                    else
                    {
                        Debug.LogFormat("Use Gravity disabled on {0} rigidbodies", changedObjectsCount);
                        consoleOutput = string.Format("Use Gravity disabled on {0} rigidbodies", changedObjectsCount);
                    }
                }
                else
                {
                    Debug.LogFormat("No changes made");
                    consoleOutput = "No changes made";
                }
            }
        }

        static void EnableUseGravity()
        {
            GameObject[] objs = Selection.gameObjects;
            //Here we have the gameobjects selected.
            int numberOfTransforms = objs.Length;
            bool showProgressBar = numberOfTransforms >= SelectionThresholdForProgressBar;
            int changedObjectsCount = 0;

            try
            {
                for (int i = 0; i < numberOfTransforms; i++)
                {

                    if (showProgressBar)
                    {
                        EditorUtility.DisplayProgressBar("Enable Use Gravity", "Enabling Use Gravity (" + i + "/" + numberOfTransforms + " parent objects)",
                            (float)i / (float)numberOfTransforms);
                    }

                    Transform[] allTransforms;

                    //Do we want to enable Use Gravity on child objects also?
                    if (includeChildrenRigidbodies)
                    {
                        allTransforms = objs[i].GetComponentsInChildren<Transform>();
                    }
                    else
                    {
                        allTransforms = objs[i].GetComponents<Transform>();
                    }

                    foreach (Transform childTransform in allTransforms)
                    {                        
                        if (useSearch)
                        {
                            bool matchedSearch = false;

                            if (sType == SEARCHTYPE.NAME)
                            {
                                if (searchCaseSentive)
                                {
                                    switch (op)
                                    {
                                        case SEARCHOPTIONS.STARTS_WITH:
                                            if (childTransform.name.StartsWith(searchString))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;

                                        case SEARCHOPTIONS.CONTAINS:
                                            if (childTransform.name.Contains(searchString))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;

                                        case SEARCHOPTIONS.ENDS_WITH:
                                            if (childTransform.name.EndsWith(searchString))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;
                                    }
                                }
                                else
                                {
                                    switch (op)
                                    {
                                        case SEARCHOPTIONS.STARTS_WITH:
                                            if (childTransform.name.ToUpper().StartsWith(searchString.ToUpper()))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;

                                        case SEARCHOPTIONS.CONTAINS:
                                            if (childTransform.name.ToUpper().Contains(searchString.ToUpper()))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;

                                        case SEARCHOPTIONS.ENDS_WITH:
                                            if (childTransform.name.ToUpper().EndsWith(searchString.ToUpper()))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;
                                    }
                                }
                            }

                            if (sType == SEARCHTYPE.TAG)
                            {
                                if (searchTagCaseSentive)
                                {
                                    if (childTransform.tag.ToUpper().Equals(searchTag.ToUpper()))
                                    {
                                        matchedSearch = true;
                                    }
                                }
                                else
                                {
                                    if (childTransform.CompareTag(searchTag))
                                    {
                                        matchedSearch = true;
                                    }
                                }
                            }

                            if (sType == SEARCHTYPE.LAYER)
                            {
                                if (childTransform.gameObject.layer == searchLayer)
                                {
                                    matchedSearch = true;
                                }
                            }

                            //We need to only use go as the game object if it either matches the seach, or else do everything
                            if (matchedSearch == true)
                            {
                                try
                                {
                                    Rigidbody[] allBodies;
                                    allBodies = childTransform.GetComponents<Rigidbody>();

                                    //we now have all the child rigidbodies
                                    if (allBodies.Length > 0)
                                    {
                                        foreach (Rigidbody aChild in allBodies)
                                        {
                                            //Record undo. Enable Use Gravity.
                                            Undo.RecordObject(aChild, "Enable Use Gravity");
                                            aChild.useGravity = true;
                                            changedObjectsCount++;
                                        }
                                    }
                                }
                                catch
                                {
                                    Debug.LogError("Unable to get rigidbodies on object named : '" + childTransform.name + "'");
                                }
                            }
                        }
                        else
                        {
                            //No search
                            try
                            {
                                Rigidbody[] allBodies;
                                allBodies = childTransform.GetComponents<Rigidbody>();

                                //we now have all the child rigidbodies
                                if (allBodies.Length > 0)
                                {
                                    foreach (Rigidbody aChild in allBodies)
                                    {
                                        //Record undo. Enable Use Gravity.
                                        Undo.RecordObject(aChild, "Enable Use Gravity");
                                        aChild.useGravity = true;
                                        changedObjectsCount++;
                                    }
                                }
                            }
                            catch
                            {
                                Debug.LogError("Unable to get rigidbodies on object named : '" + childTransform.name + "'");
                            }
                        }
                    }
                }
            }
            finally
            {
                if (showProgressBar)
                {
                    EditorUtility.ClearProgressBar();
                }

                if (changedObjectsCount > 0)
                {
                    if (changedObjectsCount == 1)
                    {
                        Debug.LogFormat("Use Gravity enabled on 1 rigidbody");
                        consoleOutput = "Use Gravity enabled on 1 rigidbody";
                    }
                    else
                    {
                        Debug.LogFormat("Use Gravity enabled on {0} rigidbodies", changedObjectsCount);
                        consoleOutput = string.Format("Use Gravity enabled on {0} rigidbodies", changedObjectsCount);
                    }
                }
                else
                {
                    Debug.LogFormat("No changes made");
                    consoleOutput = "No changes made";
                }
            }
        }

        static void AddRigidBody()
        {
            GameObject[] objs = Selection.gameObjects;
            //Here we have the gameobjects selected.
            int numberOfTransforms = objs.Length;
            bool showProgressBar = numberOfTransforms >= SelectionThresholdForProgressBar;
            int changedObjectsCount = 0;

            try
            {
                for (int i = 0; i < numberOfTransforms; i++)
                {

                    if (showProgressBar)
                    {
                        EditorUtility.DisplayProgressBar("Add RigidBody", "Adding RigidBody (" + i + "/" + numberOfTransforms + " parent objects)",
                            (float)i / (float)numberOfTransforms);
                    }


                    Transform[] allTransforms;

                    //Do we want to add rigidbodies to child objects also?
                    if (includeChildrenRigidbodies)
                    {
                        allTransforms = objs[i].GetComponentsInChildren<Transform>();
                    }
                    else
                    {
                        allTransforms = objs[i].GetComponents<Transform>();
                    }


                    foreach (Transform childTransform in allTransforms)
                    {
                        if (useSearch)
                        {
                            bool matchedSearch = false;

                            if (sType == SEARCHTYPE.NAME)
                            {
                                if (searchCaseSentive)
                                {
                                    switch (op)
                                    {
                                        case SEARCHOPTIONS.STARTS_WITH:
                                            if (childTransform.name.StartsWith(searchString))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;

                                        case SEARCHOPTIONS.CONTAINS:
                                            if (childTransform.name.Contains(searchString))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;

                                        case SEARCHOPTIONS.ENDS_WITH:
                                            if (childTransform.name.EndsWith(searchString))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;
                                    }
                                }
                                else
                                {
                                    switch (op)
                                    {
                                        case SEARCHOPTIONS.STARTS_WITH:
                                            if (childTransform.name.ToUpper().StartsWith(searchString.ToUpper()))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;

                                        case SEARCHOPTIONS.CONTAINS:
                                            if (childTransform.name.ToUpper().Contains(searchString.ToUpper()))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;

                                        case SEARCHOPTIONS.ENDS_WITH:
                                            if (childTransform.name.ToUpper().EndsWith(searchString.ToUpper()))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;
                                    }
                                }
                            }

                            if (sType == SEARCHTYPE.TAG)
                            {
                                if (searchTagCaseSentive)
                                {
                                    if (childTransform.tag.ToUpper().Equals(searchTag.ToUpper()))
                                    {
                                        matchedSearch = true;
                                    }
                                }
                                else
                                {
                                    if (childTransform.CompareTag(searchTag))
                                    {
                                        matchedSearch = true;
                                    }
                                }
                            }

                            if (sType == SEARCHTYPE.LAYER)
                            {
                                if (childTransform.gameObject.layer == searchLayer)
                                {
                                    matchedSearch = true;
                                }
                            }

                            //We need to only use go as the game object if it either matches the seach, or else do everything
                            if (matchedSearch == true)
                            {
                                try
                                {
                                    var scriptToAdd = childTransform.gameObject.GetComponent<Rigidbody>();
                                    //Only add rigidbody to transform if it isn't there already.
                                    if (scriptToAdd == null)
                                    {
                                        try
                                        {
                                            //Record undo. Add rigidbody
                                            Undo.AddComponent(childTransform.gameObject, typeof(Rigidbody));
                                            changedObjectsCount++;
                                        }
                                        catch
                                        {
                                            Debug.LogError("Unable to add rigidbody to object named : '" + childTransform.name + "'");
                                        }
                                    }
                                }
                                catch
                                {
                                    Debug.LogError("Unable to add rigidbody to object named : '" + childTransform.name + "'");
                                }
                            }
                        }
                        else
                        {
                            //No search filtering
                            try
                            {
                                var scriptToAdd = childTransform.gameObject.GetComponent<Rigidbody>();
                                //Only add rigidbody to transform if it isn't there already.
                                if (scriptToAdd == null)
                                {
                                    try
                                    {
                                        //Record undo. Add rigidbody
                                        Undo.AddComponent(childTransform.gameObject, typeof(Rigidbody));
                                        changedObjectsCount++;
                                    }
                                    catch
                                    {
                                        Debug.LogError("Unable to add rigidbody to object named : '" + childTransform.name + "'");
                                    }
                                }
                            }
                            catch
                            {
                                Debug.LogError("Unable to add rigidbody to object named : '" + childTransform.name + "'");
                            }
                        }
                    }
                }
            }
            finally
            {
                if (showProgressBar)
                {
                    EditorUtility.ClearProgressBar();
                }

                if (changedObjectsCount > 0)
                {
                    if (changedObjectsCount == 1)
                    {
                        Debug.LogFormat("Rigidbody added to 1 Gameobject");
                        consoleOutput = "Rigidbody added to 1 Gameobject";
                    }
                    else
                    {
                        Debug.LogFormat("Rigidbody added to {0} Gameobjects", changedObjectsCount);
                        consoleOutput = string.Format("Rigidbody added to {0} Gameobjects", changedObjectsCount);
                    }
                }
                else
                {
                    Debug.LogFormat("No Rigidbodies added");
                    consoleOutput = "No Rigidbodies added";
                }
            }
        }

        static void RemoveRigidBody()
        {
            GameObject[] objs = Selection.gameObjects;
            //Here we have the gameobjects selected.
            int numberOfTransforms = objs.Length;
            bool showProgressBar = numberOfTransforms >= SelectionThresholdForProgressBar;
            int changedObjectsCount = 0;

            try
            {
                for (int i = 0; i < numberOfTransforms; i++)
                {

                    if (showProgressBar)
                    {
                        EditorUtility.DisplayProgressBar("Remove Rigidbody", "Removing Rigidbody (" + i + "/" + numberOfTransforms + " parent objects)",
                            (float)i / (float)numberOfTransforms);
                    }

                    Transform[] allTransforms;

                    if (includeChildrenRigidbodies)
                    {
                        allTransforms = objs[i].GetComponentsInChildren<Transform>();
                    }
                    else
                    {
                        allTransforms = objs[i].GetComponents<Transform>();
                    }


                    foreach (Transform childTransform in allTransforms)
                    {
                        if (useSearch)
                        {
                            bool matchedSearch = false;

                            if (sType == SEARCHTYPE.NAME)
                            {
                                if (searchCaseSentive)
                                {
                                    switch (op)
                                    {
                                        case SEARCHOPTIONS.STARTS_WITH:
                                            if (childTransform.name.StartsWith(searchString))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;

                                        case SEARCHOPTIONS.CONTAINS:
                                            if (childTransform.name.Contains(searchString))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;

                                        case SEARCHOPTIONS.ENDS_WITH:
                                            if (childTransform.name.EndsWith(searchString))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;
                                    }
                                }
                                else
                                {
                                    switch (op)
                                    {
                                        case SEARCHOPTIONS.STARTS_WITH:
                                            if (childTransform.name.ToUpper().StartsWith(searchString.ToUpper()))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;

                                        case SEARCHOPTIONS.CONTAINS:
                                            if (childTransform.name.ToUpper().Contains(searchString.ToUpper()))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;

                                        case SEARCHOPTIONS.ENDS_WITH:
                                            if (childTransform.name.ToUpper().EndsWith(searchString.ToUpper()))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;
                                    }
                                }
                            }

                            if (sType == SEARCHTYPE.TAG)
                            {
                                if (searchTagCaseSentive)
                                {
                                    if (childTransform.tag.ToUpper().Equals(searchTag.ToUpper()))
                                    {
                                        matchedSearch = true;
                                    }
                                }
                                else
                                {
                                    if (childTransform.CompareTag(searchTag))
                                    {
                                        matchedSearch = true;
                                    }
                                }
                            }

                            if (sType == SEARCHTYPE.LAYER)
                            {
                                if (childTransform.gameObject.layer == searchLayer)
                                {
                                    matchedSearch = true;
                                }
                            }

                            //We need to only use go as the game object if it either matches the seach, or else do everything
                            if (matchedSearch == true)
                            {
                                try
                                {
                                    try
                                    {
                                        Rigidbody[] scriptsToRemove = childTransform.gameObject.GetComponents<Rigidbody>();
                                        //get a list of all rigidbodies attached to the current transform (in case there are more than one)
                                        foreach (Rigidbody aComp in scriptsToRemove)
                                        {
                                            if (aComp != null)
                                            {
                                                if (childTransform.gameObject.CanDestroy(aComp.GetType()))
                                                {
                                                    //Record undo. Remove rigidbody
                                                    Undo.DestroyObjectImmediate(aComp);
                                                    changedObjectsCount++;
                                                }
                                                else
                                                {
                                                    Debug.Log("Unable to remove Rigidbody on object named : " + childTransform.name + " due to dependencies");
                                                }
                                            }
                                        }
                                    }
                                    catch
                                    {
                                        Debug.LogError("Unable to remove rigidbody on object named : '" + childTransform.name + "'");
                                    }
                                }
                                catch
                                {
                                    Debug.LogError("Unable to remove rigidbody on object named : '" + childTransform.name + "'");
                                }
                            }
                        }
                        else
                        {
                            //No search filter
                            try
                            {
                                try
                                {
                                    
                                    Rigidbody[] scriptsToRemove = childTransform.gameObject.GetComponents<Rigidbody>();
                                    //get a list of all rigidbodies attached to the current transform (in case there are more than one)
                                    foreach (Rigidbody aComp in scriptsToRemove)
                                    {
                                        if (aComp != null)
                                        {
                                            if (childTransform.gameObject.CanDestroy(aComp.GetType()))
                                            {
                                                //Record undo. Remove rigidbody
                                                Undo.DestroyObjectImmediate(aComp);
                                                changedObjectsCount++;
                                            }
                                            else
                                            {
                                                Debug.Log("Unable to remove Rigidbody on object named : " + childTransform.name + " due to dependencies");
                                            }
                                        }
                                    }
                                }
                                catch
                                {
                                    Debug.LogError("Unable to remove rigidbody on object named : '" + childTransform.name + "'");
                                }
                            }
                            catch
                            {
                                Debug.LogError("Unable to remove rigidbody on object named : '" + childTransform.name + "'");
                            }
                        }
                    }
                }
            }
            finally
            {
                //Output results
                if (showProgressBar)
                {
                    EditorUtility.ClearProgressBar();
                }

                if (changedObjectsCount > 0)
                {
                    if (changedObjectsCount == 1)
                    {
                        Debug.LogFormat("1 rigidbody removed");
                        consoleOutput = "1 rigidbody removed";
                    }
                    else
                    {
                        Debug.LogFormat("{0} rigidbodies removed", changedObjectsCount);
                        consoleOutput = string.Format("{0} rigidbodies removed", changedObjectsCount);
                    }
                }
                else
                {
                    Debug.LogFormat("No rigidbodies removed");
                    consoleOutput = "No rigidbodies removed";
                }

            }
        }

        #endregion

        #region Scripts

        /// <summary>
        /// These are the functions that perform the scripts tool actions.
        /// </summary>

        static void AddScripts()
        {
            GameObject[] objs = Selection.gameObjects;
            //Here we have the gameobjects selected.
            int numberOfTransforms = objs.Length;
            bool showProgressBar = numberOfTransforms >= SelectionThresholdForProgressBar;
            int changedObjectsCount = 0;

            try
            {
                for (int i = 0; i < numberOfTransforms; i++)
                {

                    if (showProgressBar)
                    {
                        EditorUtility.DisplayProgressBar("Add Script", "Adding script (" + i + "/" + numberOfTransforms + " parent objects)",
                            (float)i / (float)numberOfTransforms);
                    }

                    Transform[] allTransforms;

                    //Are we possibly adding scripts to child objects?
                    if (includeChildrenScripts)
                    {
                        allTransforms = objs[i].GetComponentsInChildren<Transform>();
                    }
                    else
                    {
                        allTransforms = objs[i].GetComponents<Transform>();
                    }

                    foreach (Transform childTransform in allTransforms)
                    {
                        if (useSearch)
                        {
                            bool matchedSearch = false;

                            if (sType == SEARCHTYPE.NAME)
                            {
                                if (searchCaseSentive)
                                {
                                    switch (op)
                                    {
                                        case SEARCHOPTIONS.STARTS_WITH:
                                            if (childTransform.name.StartsWith(searchString))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;

                                        case SEARCHOPTIONS.CONTAINS:
                                            if (childTransform.name.Contains(searchString))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;

                                        case SEARCHOPTIONS.ENDS_WITH:
                                            if (childTransform.name.EndsWith(searchString))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;
                                    }
                                }
                                else
                                {
                                    switch (op)
                                    {
                                        case SEARCHOPTIONS.STARTS_WITH:
                                            if (childTransform.name.ToUpper().StartsWith(searchString.ToUpper()))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;

                                        case SEARCHOPTIONS.CONTAINS:
                                            if (childTransform.name.ToUpper().Contains(searchString.ToUpper()))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;

                                        case SEARCHOPTIONS.ENDS_WITH:
                                            if (childTransform.name.ToUpper().EndsWith(searchString.ToUpper()))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;
                                    }
                                }
                            }

                            if (sType == SEARCHTYPE.TAG)
                            {
                                if (searchTagCaseSentive)
                                {
                                    if (childTransform.tag.ToUpper().Equals(searchTag.ToUpper()))
                                    {
                                        matchedSearch = true;
                                    }
                                }
                                else
                                {
                                    if (childTransform.CompareTag(searchTag))
                                    {
                                        matchedSearch = true;
                                    }
                                }
                            }

                            if (sType == SEARCHTYPE.LAYER)
                            {
                                if (childTransform.gameObject.layer == searchLayer)
                                {
                                    matchedSearch = true;
                                }
                            }

                            //We need to only use go as the game object if it either matches the seach, or else do everything
                            if (matchedSearch == true)
                            {
                                try
                                {
                                    var scriptToAdd = childTransform.gameObject.GetComponent(MultiEditTools.scriptToAdd.GetClass());
                                    //only add the script if it doesn't exist on the transform.
                                    if (scriptToAdd == null)
                                    {
                                        try
                                        {
                                            //Record Undo statement. Add the script.
                                            Undo.AddComponent(childTransform.gameObject, MultiEditTools.scriptToAdd.GetClass());
                                            changedObjectsCount++;
                                        }
                                        catch
                                        {
                                            //Sometimes certain script types are incompatible.
                                            Debug.LogError("Unable to add script to object named : '" + childTransform.name + "'");
                                        }
                                    }
                                }
                                catch
                                {
                                    Debug.LogError("Unable to add script to object named : '" + childTransform.name + "'");
                                }
                            }
                        }
                        else
                        {
                            //No search filtering. Add script to transform.
                            try
                            {
                                var scriptToAdd = childTransform.gameObject.GetComponent(MultiEditTools.scriptToAdd.GetClass());
                                //Only add the script if it isn't already in place.
                                if (scriptToAdd == null)
                                {
                                    try
                                    {
                                        //Record undo. Add script.
                                        Undo.AddComponent(childTransform.gameObject, MultiEditTools.scriptToAdd.GetClass());
                                        changedObjectsCount++;
                                    }
                                    catch
                                    {
                                        Debug.LogError("Unable to add script to object named : '" + childTransform.name + "'");
                                    }
                                }
                            }
                            catch
                            {
                                Debug.LogError("Unable to add script to object named : '" + childTransform.name + "'");
                            }
                        }
                    }
                }
            }
            finally
            {
                //Show Results
                if (showProgressBar)
                {
                    EditorUtility.ClearProgressBar();
                }

                if (changedObjectsCount > 0)
                {
                    if (changedObjectsCount == 1)
                    {
                        Debug.LogFormat("Script added to 1 Gameobject");
                        consoleOutput = "Script added to 1 Gameobject";
                    }
                    else
                    {
                        Debug.LogFormat("Script added to {0} Gameobjects", changedObjectsCount);
                        consoleOutput = string.Format("Script added to {0} Gameobjects", changedObjectsCount);
                    }
                }
                else
                {
                    Debug.LogFormat("No scripts added");
                    consoleOutput = "No scripts added";
                }

            }
        }

        static void RemoveScripts()
        {
            GameObject[] objs = Selection.gameObjects;
            //Here we have the gameobjects selected.
            int numberOfTransforms = objs.Length;
            bool showProgressBar = numberOfTransforms >= SelectionThresholdForProgressBar;
            int changedObjectsCount = 0;

            try
            {
                for (int i = 0; i < numberOfTransforms; i++)
                {

                    if (showProgressBar)
                    {
                        EditorUtility.DisplayProgressBar("Remove Script", "Removing script (" + i + "/" + numberOfTransforms + " parent objects)",
                            (float)i / (float)numberOfTransforms);
                    }

                    Transform[] allTransforms;

                    //Must we possibly remove scripts from child objects also?
                    if (includeChildrenScripts)
                    {
                        allTransforms = objs[i].GetComponentsInChildren<Transform>();
                    }
                    else
                    {
                        allTransforms = objs[i].GetComponents<Transform>();
                    }

                    foreach (Transform childTransform in allTransforms)
                    {
                        if (useSearch)
                        {
                            bool matchedSearch = false;

                            if (sType == SEARCHTYPE.NAME)
                            {
                                if (searchCaseSentive)
                                {
                                    switch (op)
                                    {
                                        case SEARCHOPTIONS.STARTS_WITH:
                                            if (childTransform.name.StartsWith(searchString))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;

                                        case SEARCHOPTIONS.CONTAINS:
                                            if (childTransform.name.Contains(searchString))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;

                                        case SEARCHOPTIONS.ENDS_WITH:
                                            if (childTransform.name.EndsWith(searchString))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;
                                    }
                                }
                                else
                                {
                                    switch (op)
                                    {
                                        case SEARCHOPTIONS.STARTS_WITH:
                                            if (childTransform.name.ToUpper().StartsWith(searchString.ToUpper()))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;

                                        case SEARCHOPTIONS.CONTAINS:
                                            if (childTransform.name.ToUpper().Contains(searchString.ToUpper()))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;

                                        case SEARCHOPTIONS.ENDS_WITH:
                                            if (childTransform.name.ToUpper().EndsWith(searchString.ToUpper()))
                                            {
                                                matchedSearch = true;
                                            }
                                            break;
                                    }
                                }
                            }

                            if (sType == SEARCHTYPE.TAG)
                            {
                                if (searchTagCaseSentive)
                                {
                                    if (childTransform.tag.ToUpper().Equals(searchTag.ToUpper()))
                                    {
                                        matchedSearch = true;
                                    }
                                }
                                else
                                {
                                    if (childTransform.CompareTag(searchTag))
                                    {
                                        matchedSearch = true;
                                    }
                                }
                            }

                            if (sType == SEARCHTYPE.LAYER)
                            {
                                if (childTransform.gameObject.layer == searchLayer)
                                {
                                    matchedSearch = true;
                                }
                            }

                            //We need to only use go as the game object if it either matches the seach, or else do everything
                            if (matchedSearch == true)
                            {
                                try
                                {
                                    try
                                    {
                                        Component[] scriptsToRemove = childTransform.gameObject.GetComponents(scriptToRemove.GetClass());
                                        //try to get all components of the current script type for removal.
                                        foreach (Component aComp in scriptsToRemove)
                                        {
                                            if (aComp != null)
                                            {
                                                if (childTransform.gameObject.CanDestroy(aComp.GetType()))
                                                {
                                                    //Record Undo statements. Remove script.
                                                    Undo.DestroyObjectImmediate(aComp);
                                                    changedObjectsCount++;
                                                }
                                                else
                                                {
                                                    Debug.Log("Unable to remove script on object named : " + childTransform.name + " due to dependencies");
                                                }
                                            }
                                        }
                                    }
                                    catch
                                    {
                                        Debug.LogError("Unable to remove script on object named : '" + childTransform.name + "'");
                                    }
                                }
                                catch
                                {
                                    Debug.LogError("Unable to remove script on object named : '" + childTransform.name + "'");
                                }
                            }
                        }
                        else
                        {
                            //No search filtering. 
                            try
                            {
                                try
                                {                                    
                                    Component[] scriptsToRemove = childTransform.gameObject.GetComponents(scriptToRemove.GetClass());
                                    //Get all copies of the script to remove. 
                                    foreach (Component aComp in scriptsToRemove)
                                    {
                                        if (aComp != null)
                                        {
                                            if (childTransform.gameObject.CanDestroy(aComp.GetType()))
                                            {
                                                //Record undo. Remove script(s).
                                                Undo.DestroyObjectImmediate(aComp);
                                                changedObjectsCount++;
                                            }
                                            else
                                            {
                                                Debug.Log("Unable to remove script on object named : " + childTransform.name + " due to dependencies");
                                            }
                                        }
                                    }
                                }
                                catch
                                {
                                    Debug.LogError("Unable to remove script on object named : '" + childTransform.name + "'");
                                }
                            }
                            catch
                            {
                                Debug.LogError("Unable to remove script on object named : '" + childTransform.name + "'");
                            }
                        }
                    }
                }
            }
            finally
            {
                //Output results
                if (showProgressBar)
                {
                    EditorUtility.ClearProgressBar();
                }

                if (changedObjectsCount > 0)
                {
                    if (changedObjectsCount == 1)
                    {
                        Debug.LogFormat("1 script removed");
                        consoleOutput = "1 script removed";
                    }
                    else
                    {
                        Debug.LogFormat("{0} scripts removed", changedObjectsCount);
                        consoleOutput = string.Format("{0} scripts removed", changedObjectsCount);
                    }
                }
                else
                {
                    Debug.LogFormat("No scripts removed");
                    consoleOutput = "No scripts removed";
                }

            }
        }

        static void AddRemoveScripts()
        {
            //We either add or remove the scripts, based on what is selected.
            if (addScript)
                AddScripts();
            if (removeScript)
                RemoveScripts();
        }

        #endregion       
    }
    #endregion

    #region ColliderAddWindow

    /// <summary>
    /// This code is for the collider choice window that opens.
    /// </summary>

    public class ColliderWindow : EditorWindow
    {
        //Variables used.
        Vector2 scrollPos;
        public int colliderMode = 0;

        //This is the main GUI window 
        private void OnGUI()
        {            
            if (colliderMode == 1)
            {
                //Remove colliders
                GUI.color = new Color(1f, 0.5f, 0.31f);
            }
            else
            {
                //Add colliders
                GUI.color = Color.green;
            }

            try
            {
                string sendCommand = string.Empty;

                EditorGUILayout.BeginVertical();
                scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

                EditorGUILayout.Space();
                EditorGUILayout.Space();

                //Choose our collider to add/remove, send the event to the other window, and close this one.
                if (this != null)
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        GUILayout.FlexibleSpace();

                        if (GUILayout.Button("Sphere Collider", GUILayout.Width(160), GUILayout.Height(20)))
                        {
                            sendCommand = "Sphere";
                            SendCommand(sendCommand);
                           Close();                            
                        }
                        GUILayout.FlexibleSpace();

                        if (GUILayout.Button("Box Collider", GUILayout.Width(160), GUILayout.Height(20)))
                        {
                            sendCommand = "Box";
                            SendCommand(sendCommand);                            
                            Close();
                        }
                        GUILayout.FlexibleSpace();
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.Space();

                //Choose our collider to add/remove, send the event to the other window, and close this one.
                if (this != null)
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        GUILayout.FlexibleSpace();

                        if (GUILayout.Button("Capsule Collider", GUILayout.Width(160), GUILayout.Height(20)))
                        {
                            sendCommand = "Capsule";
                            SendCommand(sendCommand);
                            Close();
                        }
                        GUILayout.FlexibleSpace();

                        if (GUILayout.Button("Mesh Collider", GUILayout.Width(160), GUILayout.Height(20)))
                        {
                            sendCommand = "Mesh";
                            SendCommand(sendCommand);
                            Close();
                        }
                        GUILayout.FlexibleSpace();
                    }
                    EditorGUILayout.EndHorizontal();
                }

                if (this != null)
                {
                    EditorGUILayout.EndScrollView();
                    EditorGUILayout.EndVertical();
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError(ex.Message);
            }
        }

        //This will send the type of collider that we are adding/removing to the other window, which will perform the action
        void SendCommand(string sendCommand)
        {
            EditorWindow win = GetWindow<MultiEditTools>();            
            win.SendEvent(EditorGUIUtility.CommandEvent(sendCommand));
        }
    }
    #endregion    

    #region HelperClasses
    static class GameObjectExtensions
    {
        private static bool Requires(Type obj, Type requirement)
        {
            //also check for m_Type1 and m_Type2 if required
            return Attribute.IsDefined(obj, typeof(RequireComponent)) &&
                   Attribute.GetCustomAttributes(obj, typeof(RequireComponent)).OfType<RequireComponent>()
                   .Any(rc => rc.m_Type0.IsAssignableFrom(requirement));
        }

        internal static bool CanDestroy(this GameObject go, Type t)
        {
            return !go.GetComponents<Component>().Any(c => Requires(c.GetType(), t));
        }
    }

    class RemoveEmptyScripts : MonoBehaviour
    {        
        public static void RemoveMissingScriptReferences()
        {
            var go = FindObjectsOfType<GameObject>();

            foreach (var g in go)
            {
                FindInGo(g);
            }

            AssetDatabase.SaveAssets();
        }

        private static void FindInGo(GameObject g)
        {
            var components = g.GetComponents<Component>();

            var r = 0;

            for (var i = 0; i < components.Length; i++)
            {
                if (components[i] != null) continue;                

                var s = g.name;
                var t = g.transform;
                while (t.parent != null)
                {
                    s = t.parent.name + "/" + s;
                    t = t.parent;
                }
             
                var serializedObject = new SerializedObject(g); 
                var prop = serializedObject.FindProperty("m_Component");                              
                prop.DeleteArrayElementAtIndex(i - r);              
                r++;
                serializedObject.ApplyModifiedProperties();                                                
            }


            foreach (Transform childT in g.transform)
            {
                FindInGo(childT.gameObject);
            }      
        }
       
    }



    #endregion

#endif
}