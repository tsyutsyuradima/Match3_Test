using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Game
{
    // Менеджер обектов на сцене (в сетке)
    public class MatchObjectsManager : MonoBehaviour
    {
        public MatchElement MatchElementPrefab;
        List<MatchElement> reservedObjectsPool = new List<MatchElement>();
        MatchElement[,] currentMatchObjects;
        public bool isSwiping = false;

        MatchElement selectedElement;
        
        public void ReserveMemoryForAllObjects(int x, int y)
        {
            if (currentMatchObjects != null)
            {
                foreach (MatchElement tmp in currentMatchObjects)
                {
                    if (tmp != null)
                    {
                        tmp.transform.localScale = Vector3.zero;
                        reservedObjectsPool.Add(tmp);
                    }
                }
            }
            currentMatchObjects = null;
            currentMatchObjects = new MatchElement[x, y];
        }

        public void CreateMatchElement(Color color, int x, int y, Transform parent)
        {
            if (reservedObjectsPool.Count == 0)
            {
                MatchElement tmp = Instantiate(MatchElementPrefab, parent);
                tmp.Initialize(x, y, color);
                tmp.OnClickEvent += OnClick;
                tmp.OnUpSwipeEvent += OnUpSwipe;
                tmp.OnDownSwipeEvent += OnDownSwipe;
                tmp.OnLeftSwipeEvent += OnLeftSwipe;
                tmp.OnRightSwipeEvent += OnRightSwipe;
                tmp.OnHideElement += OnHideElement;
                currentMatchObjects[x, y] = tmp;
            }
            else
            {
                MatchElement tmp = reservedObjectsPool[0];
                tmp.Initialize(x, y, color);
                tmp.transform.SetParent(parent, false);
                tmp.transform.localPosition = Vector3.zero;
                currentMatchObjects[x, y] = tmp;
                reservedObjectsPool.RemoveAt(0);
            }
        }

        void OnHideElement(SwipeComponent obj)
        {
            reservedObjectsPool.Add((MatchElement)obj);
        }



        void OnClick(SwipeComponent obj)
        {
            if (!isSwiping)
            {
                MatchElement clickedElement = (MatchElement)obj;
                if (selectedElement == null)
                {
                    selectedElement = clickedElement;
                    selectedElement.StartTweenScale();
                }
                else
                {
                    // одинаковый ряд, проверяем соседство в колонке     
                    if (selectedElement.match.Y == clickedElement.match.Y && (Mathf.Abs(selectedElement.match.X - clickedElement.match.X) == 1))
                    {
                        SwipeObjects(selectedElement, clickedElement);
                    }
                    // одинаковая колонка, проверяем соседство в ряду     
                    else if (selectedElement.match.X == clickedElement.match.X && (Mathf.Abs(selectedElement.match.Y - clickedElement.match.Y) == 1))
                    {
                        SwipeObjects(selectedElement, clickedElement);
                    }
                    // нет соседства, скидываем выбор с первой фишки     
                    else
                    {
                        selectedElement.StopTween();
                        selectedElement = clickedElement;
                        selectedElement.StartTweenScale();
                    }
                }
            }
        }

        void OnRightSwipe(SwipeComponent obj)
        {
            if (!isSwiping)
            {
                MatchElement from = (MatchElement)obj;
                // убедимся, что фишка не выходит за пределы поля    
                if (from.match.X < GlobalGameSettings.Instance.Col)
                {
                    //достаем обект с которим будем матчить.
                    MatchElement to = currentMatchObjects[from.match.X + 1, from.match.Y];
                    SwipeObjects(from, to);
                }
            }
        }

        void OnLeftSwipe(SwipeComponent obj)
        {
            if (!isSwiping)
            {
                MatchElement from = (MatchElement)obj;
                // убедимся, что фишка не выходит за пределы поля    
                if (from.match.X > 0)
                {
                    //достаем обект с которим будем матчить.
                    MatchElement to = currentMatchObjects[from.match.X - 1, from.match.Y];
                    SwipeObjects(from, to);
                }
            }
        }

        void OnDownSwipe(SwipeComponent obj)
        {
            if (!isSwiping)
            {
                MatchElement from = (MatchElement)obj;
                // убедимся, что фишка не выходит за пределы поля    
                if (from.match.Y > 0)
                {
                    //достаем обект с которим будем матчить.
                    MatchElement to = currentMatchObjects[from.match.X, from.match.Y - 1];
                    SwipeObjects(from, to);
                }
            }
        }

        void OnUpSwipe(SwipeComponent obj)
        {
            if (!isSwiping)
            {
                MatchElement from = (MatchElement)obj;
                // убедимся, что фишка не выходит за пределы поля    
                if (from.match.Y < GlobalGameSettings.Instance.Row)
                {
                    //достаем обект с которим будем матчить.
                    MatchElement to = currentMatchObjects[from.match.X, from.match.Y + 1];
                    SwipeObjects(from, to);
                }
            }
        }



        void SwipeObjects(MatchElement from, MatchElement to)
        {
            isSwiping = true;
            if (selectedElement != null)
                selectedElement.StopTween();
            selectedElement = null;

            Match3SceneManager.Instance.PlaySwipeSound();

            // отсоеденяем от елемента в гриду
            to.transform.SetParent(gameObject.transform);
            from.transform.SetParent(gameObject.transform);
            Vector3 toPos = to.transform.position;
            Vector3 fromPos = from.transform.position;

            //свайпаем
            if (Match3SceneManager.Instance.TryToMatch(from.match, to.match)) // оправляем в клас проверки
            {
                //обмениваем значения X и Y
                int tmpX = from.match.X;
                int tmpY = from.match.Y;
                from.match.X = to.match.X;
                from.match.Y = to.match.Y;
                to.match.X = tmpX;
                to.match.Y = tmpY;

                currentMatchObjects[to.match.X, to.match.Y] = to;
                currentMatchObjects[from.match.X, from.match.Y] = from;

                //двигаем фишки
                to.transform.DOMove(fromPos, 0.2f);
                from.transform.DOMove(toPos, 0.2f).OnComplete(() =>
                {
                    //возврощаем в грид
                    to.transform.SetParent(Match3SceneManager.Instance.GetPositionInGrid(to.match.X, to.match.Y));
                    from.transform.SetParent(Match3SceneManager.Instance.GetPositionInGrid(from.match.X, from.match.Y));

                    // даем знак прощитать текущую комбинацию и ждем ответ с масивом обектов которих нужно будет удалить
                    Match3SceneManager.Instance.FindRemoveMatches(); 
                });
            }
            else
            {
                //двигаем фишки
                to.transform.DOMove(fromPos, 0.2f).SetLoops(2, LoopType.Yoyo);
                from.transform.DOMove(toPos, 0.2f).SetLoops(2, LoopType.Yoyo).OnComplete(() =>
                {
                    //возврощаем в грид
                    to.transform.SetParent(Match3SceneManager.Instance.GetPositionInGrid(to.match.X, to.match.Y));
                    from.transform.SetParent(Match3SceneManager.Instance.GetPositionInGrid(from.match.X, from.match.Y));
                    isSwiping = false;
                });
            }
        }

        public void RemoveMatches(List<MatchBase> matches)
        {
            foreach (MatchBase match in matches)
            {
                if (currentMatchObjects[match.X, match.Y] != null)
                {
                    Match3SceneManager.Instance.PlayBubbleSound();
                    currentMatchObjects[match.X, match.Y].HideElement();
                    currentMatchObjects[match.X, match.Y] = null;
                }
            }
            foreach (MatchBase match in matches)
            {
                affectAbove(match.X, match.Y);
            }

            // немного ждем и говорим прощитать новие фишки
            StartCoroutine(WaitSyncAndCreateNew());
        }

        //заставляет все фишки над переданной в функцию двигаться вниз   
        public void affectAbove(int x, int y)
        {
            for (int i = 0; i < GlobalGameSettings.Instance.Row-1; i++)
            {
                if (currentMatchObjects[x, i] == null && currentMatchObjects[x, i+1] != null)
                {
                    currentMatchObjects[x, i+1].transform.SetParent(gameObject.transform);
                    currentMatchObjects[x, i+1].MoveTo(Match3SceneManager.Instance.GetPositionInGrid(x, i));
                    currentMatchObjects[x, i+1].match.Y--;
                    currentMatchObjects[x, i] = currentMatchObjects[x, i+1];
                    currentMatchObjects[x, i+1] = null;
                }
            }
        }

        IEnumerator WaitSyncAndCreateNew()
        {
            yield return new WaitForSeconds(0.1f);
            Match3SceneManager.Instance.Sync();
            yield return new WaitForSeconds(0.5f);
            Match3SceneManager.Instance.CreateNewPieces();
        }

        public void Sync(MatchBase[,] shablon)
        {
            for (int x = 0; x < GlobalGameSettings.Instance.Col; x++)
            {
                for (int y = 0; y < GlobalGameSettings.Instance.Row; y++)
                {
                    if (shablon[x, y] != null)
                    {
                        if (currentMatchObjects[x, y] == null )
                            CreateMatchElement(shablon[x, y].MyColor, shablon[x, y].X, shablon[x, y].Y, Match3SceneManager.Instance.GetPositionInGrid(x, y));
                        currentMatchObjects[x, y].Initialize(shablon[x, y].X, shablon[x, y].Y, shablon[x, y].MyColor);
                    }
                }
            }
        }
    }
}