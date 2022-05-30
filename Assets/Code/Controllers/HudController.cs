using Snowboard.Views;
using System.Collections.Generic;

namespace Snowboard.Controllers
{
    public class HudController : GameActor
    {

        private List<BoarderController> rivals;
        private BoarderController player;

        private HudView view;

        private float raceLength;

        private bool gameStarted;

        public HudController(HudView view, float raceLength)
        {
            this.view = view;
            this.raceLength = raceLength;

            rivals = new List<BoarderController>();

            view.ShowTitle(true);
            view.ShowProgress(false);
            view.ShowFinalMessage(false);
        }

        public override void Update()
        {
            if (gameStarted)
            {
                view.Progress = player.Position.x / raceLength;

                int position = rivals.Count + 1;
                for (int i = 0; i < rivals.Count; i++)
                {
                    if (player.Position.x > rivals[i].Position.x)
                    {
                        position--;
                    }
                }

                view.SetPosition(position, rivals.Count + 1);
            }
        }


        public void AddBoarder(BoarderController boarder)
        {
            if (boarder.AiControlled)
            {
                rivals.Add(boarder);
            }
            else
            {
                player = boarder;
            }

        }

        public override void OnGameStart()
        {
            gameStarted = true;
            view.ShowProgress(true);
            view.ShowTitle(false);
        }

        public override void OnGameEnd()
        {
            gameStarted = false;
            view.ShowFinalMessage(true);
        }

        public override void OnGameReset()
        {
            view.ShowProgress(false);
            view.ShowTitle(true);
            view.ShowFinalMessage(false);
        }
    }
}