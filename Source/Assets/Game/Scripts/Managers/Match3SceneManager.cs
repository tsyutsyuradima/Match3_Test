using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    /// <summary>
    /// Основной связующий клас ... между логикой прощитивания сетки и отображением
    /// </summary>
    public class Match3SceneManager : MonoBehaviour
    {
        public static Match3SceneManager Instance;

        public GameGridController GridController;
        public ScorePanelController ScoreController;
        public MatchObjectsManager ObjectsManager;
        public GameOverPanelController GameOverPanel;
        public GameObject LoadingText;

        Match3Manager match3Manager = new Match3Manager();
        
        public AudioClip gameOver;
        public AudioClip swipe;
        public AudioClip buble;
        public AudioSource audioSource;

        void Start()
        {
            Instance = this;
            ScoreController.ClickBtnRestart += OnClickBtnRestart;

            match3Manager.OnEndGenerationElements += OnEndGenerationElements;
            match3Manager.OnUpdateScoreEvent += OnUpdateScoreEvent;
            match3Manager.OnGameOverEvent += OnGameOverEvent;
            match3Manager.OnCreateNewEvent += OnCreateNewEvent;
            match3Manager.OnRemoveMatchesEvent += OnRemoveMatchesEvent;
            match3Manager.OnContinueGamePlayEvent += OnContinueGamePlayEvent;

            GridController.GenerateNewGrid(GlobalGameSettings.Instance.Col, GlobalGameSettings.Instance.Row);
            ObjectsManager.ReserveMemoryForAllObjects(GlobalGameSettings.Instance.Col, GlobalGameSettings.Instance.Row);
            StartCoroutine(match3Manager.GenerateNewList(GlobalGameSettings.Instance.Col, GlobalGameSettings.Instance.Row));
        }

        public void OnClickBtnRestart()
        {
            LoadingText.SetActive(true);
            StopAllCoroutines();
            GlobalGameSettings.Instance.Score = 0;
            ScoreController.Score.text = GlobalGameSettings.Instance.Score.ToString();

            ObjectsManager.ReserveMemoryForAllObjects(GlobalGameSettings.Instance.Col, GlobalGameSettings.Instance.Row);
            StartCoroutine(match3Manager.GenerateNewList(GlobalGameSettings.Instance.Col, GlobalGameSettings.Instance.Row));

            ObjectsManager.isSwiping = false;
        }

        private void OnEndGenerationElements(MatchBase[,] allElements)
        {
            LoadingText.SetActive(false);

            for (int x = 0; x < GlobalGameSettings.Instance.Col; x++)
            {
                for (int y = 0; y < GlobalGameSettings.Instance.Row; y++)
                {
                    Color color = allElements[x, y].MyColor;
                    Transform parent = GridController.GetTransformByIndex(x, y);
                    ObjectsManager.CreateMatchElement(color, x, y, parent);
                }
            }
        }
        public bool TryToMatch(MatchBase from, MatchBase to)
        {
            return match3Manager.MakeSwap(from.X, from.Y, to.X, to.Y);
        }
        public void FindRemoveMatches()
        {
            match3Manager.FindAndRemoveMatches();
        }
        private void OnRemoveMatchesEvent(List<MatchBase> list)
        {
            ObjectsManager.RemoveMatches(list);
        }
        public void CreateNewPieces()
        {
            match3Manager.AddNewPieces();
        }
        private void OnCreateNewEvent(List<MatchBase> list)
        {
            foreach (MatchBase item in list)
            {
                Transform parent = GridController.GetTransformByIndex(item.X, item.Y);
                ObjectsManager.CreateMatchElement(item.MyColor, item.X, item.Y, parent);
            }
            StartCoroutine(WaitAndCheck());
        }
        IEnumerator WaitAndCheck()
        {
            yield return new WaitForSeconds(0.4f);
            match3Manager.ReadyToCheck();
        }

        private void OnUpdateScoreEvent(int score)
        {
            GlobalGameSettings.Instance.Score += score;
            ScoreController.Score.text = GlobalGameSettings.Instance.Score.ToString();
        }
        private void OnContinueGamePlayEvent()
        {
            ObjectsManager.isSwiping = false;
            Sync();
        }
        private void OnGameOverEvent()
        {
            audioSource.Stop();
            audioSource.clip = gameOver;
            audioSource.Play();
            GameOverPanel.gameObject.SetActive(true);
        }
        public Transform GetPositionInGrid(int x, int y)
        {
            return GridController.GetTransformByIndex(x, y);
        }

        public void PlayBubbleSound()
        {
            audioSource.Stop();
            audioSource.clip = buble;
            audioSource.Play();
        }
        public void PlaySwipeSound()
        {
            audioSource.Stop();
            audioSource.clip = swipe;
            audioSource.Play();
        }

        void OnDestroy()
        {
            StopAllCoroutines();
            match3Manager.OnEndGenerationElements -= OnEndGenerationElements;
            match3Manager.OnUpdateScoreEvent -= OnUpdateScoreEvent;
            match3Manager.OnGameOverEvent -= OnGameOverEvent;
            match3Manager.OnCreateNewEvent -= OnCreateNewEvent;
            match3Manager.OnRemoveMatchesEvent -= OnRemoveMatchesEvent;
            match3Manager.OnContinueGamePlayEvent -= OnContinueGamePlayEvent;
        }

        public void Sync()
        {
            ObjectsManager.Sync(match3Manager.allMathElements);
        }
    }
}