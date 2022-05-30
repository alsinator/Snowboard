using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
using Snowboard.Views;

namespace Snowboard.Controllers
{
    public class GameStateController : MonoBehaviour
    {

        [SerializeField] private BoarderCatalog boarderCatalog;
        [SerializeField] private MountainCatalog mountainCatalog;

        [SerializeField] private HudView hudView; // This should be loaded too...

        [SerializeField] private Camera mainCamera;

        private MountainController mountain;
        private MountainData mountainData;

        private BoarderController player;

        private HudController hud;

        private List<GameActor> actors;


        // This should be a State Machine, but I'm already using it for the boarders
        // And not much time left
        private enum GameStates
        {
            Title,
            Game,
            End
        }

        private GameStates gameState = GameStates.Title;

        private float stateTimer;

        // With more time I'd do a better event system
        private Action OnGameEnd;
        private Action OnGameStart;
        private Action OnGameReset;


        void Start()
        {
            actors = new List<GameActor>(30);

            if (LoadMountain())
            {
                CreateBoarders();
            }

            CreateHud();
            CreateCamera();
        }

        void Update()
        {
            UpdateState();
            UpdateActors();
        }

        void UpdateState()
        {
            if (gameState == GameStates.Title && Input.GetButtonDown("Fire1"))
            {
                gameState = GameStates.Game;
                OnGameStart();
            }
            else if (gameState == GameStates.Game)
            {
                if (player.Position.x > mountainData.Length)
                {
                    stateTimer = Time.time + 3f;
                    gameState = GameStates.End;
                    OnGameEnd();
                }
            }
            else if (gameState == GameStates.End && stateTimer < Time.time)
            {
                gameState = GameStates.Title;
                OnGameReset();
            }
        }


        private void UpdateActors()
        {
            bool somethingToRemove = false;
            for (int i = 0; i < actors.Count; i++)
            {
                var controller = actors[i];
                controller.Update();

                if (controller.ReadyToRemove)
                {
                    OnGameStart -= controller.OnGameStart;
                    OnGameEnd -= controller.OnGameEnd;
                    OnGameReset -= controller.OnGameReset;

                    somethingToRemove = true;
                }
            }

            if (somethingToRemove)
            {
                actors.RemoveAll(a => a.ReadyToRemove);
            }
        }

        public void AddActor(GameActor actor)
        {
            OnGameStart += actor.OnGameStart;
            OnGameEnd += actor.OnGameEnd;
            OnGameReset += actor.OnGameReset;

            actors.Add(actor);
        }


        private bool LoadMountain()
        {
            // Lets assume we decided which mountain in some sort of menu
            mountainData = mountainCatalog.GetMountain(0);

            AssetBundle bundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, mountainData.BundleId));

            if (bundle != null)
            {
                var asset = bundle.LoadAsset<GameObject>("Mountain");
                if (asset != null)
                {
                    var mountainInstance = Instantiate(asset);
                    var mountainView = mountainInstance.GetComponent<MountainView>();
                    mountain = new MountainController(mainCamera, mountainView);

                    OnGameReset += mountain.OnGameReset;

                    bundle.Unload(false); // Everything we need is contained in the mountain prefab
                    return true;
                }
                bundle.Unload(false);
            }

            Debug.LogError("Error loading mountain");
            return false;
        }

        private void CreateBoarders()
        {

            // Player character
            // We could have selected skin in a menu
            var boarder = GetBoarderWithSkin(0);
            player = boarder;
            AddActor(boarder);

            // Rivals
            for (int i = 1; i < boarderCatalog.NumBoarders; i++)
            {
                boarder = GetBoarderWithSkin(i, UnityEngine.Random.Range(0.9f, 1.2f), true);
                AddActor(boarder);
            }
        }

        private BoarderController GetBoarderWithSkin(int skin, float speed = 1f, bool isAI = false)
        {
            var boarderView = Instantiate(boarderCatalog.BoarderPrefab).GetComponent<BoarderView>();
            boarderView.boarderSkin = boarderCatalog.GetBoarderMaterial(skin);
            var boarder = new BoarderController(isAI, speed, boarderView.GetComponent<BoarderView>(), mountain);

            return boarder;
        }

        private void CreateHud()
        {
            hud = new HudController(hudView, mountainData.Length);

            for (int i = 0; i < actors.Count; i++)
            {
                if (actors[i] is BoarderController boarder)
                {
                    hud.AddBoarder(boarder);
                }
            }

            AddActor(hud);
        }

        private void CreateCamera()
        {
            var camera = new CameraController(mainCamera, player);
            AddActor(camera);
        }

    }
}