using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Game
{
    public class MatchBase
    {
        public int X;
        public int Y;
        public Color MyColor;

        public MatchBase()
        {
        }

        public MatchBase(int x, int y, Color color)
        {
            this.X = x;
            this.Y = y;
            this.MyColor = color;
        }
    }

    public class Match3Manager
    {
        public delegate void ElementsGenerationEvent(MatchBase[,] allElements);
        public delegate void SimpleEvent();
        public delegate void ScoreEvent(int score);
        public delegate void ListEvent(List<MatchBase> list);

        public event ElementsGenerationEvent OnEndGenerationElements;
        public event SimpleEvent OnGameOverEvent;
        public event SimpleEvent OnContinueGamePlayEvent;
        public event ScoreEvent OnUpdateScoreEvent;
        public event ListEvent OnRemoveMatchesEvent;
        public event ListEvent OnCreateNewEvent;

        // All avalible colors for random selection.
        Color[] GameColors = new Color[] { Color.red, Color.green, Color.blue, Color.grey, Color.cyan, Color.yellow, Color.magenta, Color.black };
        public MatchBase[,] allMathElements;   // All elements.

        int _row;    //количество ячеек в рядке
        int _col;    //количество ячеек в столбце


        public IEnumerator GenerateNewList(int col, int row)
        {
            this._col = col;
            this._row = row;
            allMathElements = null;
            allMathElements = new MatchBase[col, row];

            while (true)
            {
                yield return null;

                GenerateRandomElements();

                // Поле, при старте игры не должно содержать линий.
                // пробуем, если на поле есть линии     
                if (ChackForMatches().Count > 0)
                    continue;
                // пробуем, если на поле нет ни одного хода     
                if (!CheckForPossibles())
                    continue;

                break;
            }

            if (OnEndGenerationElements != null)
                OnEndGenerationElements(allMathElements);
        }

        //public void GenerateNewList(int col, int row)
        //{
        //    this._col = col;
        //    this._row = row;
        //    allMathElements = null;
        //    allMathElements = new MatchBase[col, row];

        //    while (true)
        //    {
        //        GenerateRandomElements();
        //        // Поле, при старте игры не должно содержать линий.
        //        // пробуем, если на поле есть линии     
        //        if (ChackForMatches().Count > 0)
        //            continue;
        //        // пробуем, если на поле нет ни одного хода     
        //        if (!CheckForPossibles())
        //            continue;
        //        break;
        //    }

        //    if (OnEndGenerationElements != null)
        //        OnEndGenerationElements(allMathElements);
        //}

        //Создание случайно-генерируемого масива обектов.
        public void GenerateRandomElements()
        {
            for (int x = 0; x < _col; x++)
            {
                for (int y = 0; y < _row; y++)
                {
                    int randomLength = (_col + _row > 10) ? GameColors.Length : GameColors.Length/2;
                    int colorIndex = UnityEngine.Random.Range(0, randomLength);
                    allMathElements[x, y] = new MatchBase(x, y, GameColors[colorIndex]);
                }
            }
        }

        // Проверка на линии.
        // Возвращаем массив всех найденных линий   
        public List<List<MatchBase>> ChackForMatches()
        {
            List<List<MatchBase>> allMatch = new List<List<MatchBase>>();
            // поиск горизонтальных линий    
            for (int row = 0; row < _row; row++)
            {
                for (int col = 0; col < _col; col++)
                {
                    List<MatchBase> match = GetMatchHoriz(col, row);
                    if (match.Count >= 3)
                    {
                        allMatch.Add(match);
                        col += match.Count;
                    }
                }
            }
            // поиск вертикальных линий    
            for (int col = 0; col < _col; col++)
            {
                for (int row = 0; row < _row; row++)
                {
                    List<MatchBase> match = GetMatchVert(col, row);
                    if (match.Count >= 3)
                    {
                        allMatch.Add(match);
                        row += match.Count;
                    }
                }
            }
            return allMatch;
        }

        // поиск горизонтальных линий из заданной точки   
        public List<MatchBase> GetMatchHoriz(int col, int row)
        {
            List<MatchBase> match = new List<MatchBase>();
            for (int i = 0; col + i < _col; i++)
            {
                if (allMathElements[col, row].MyColor == allMathElements[col + i, row].MyColor)
                    match.Add(new MatchBase(col + i, row, allMathElements[col + i, row].MyColor));
                else
                    return match;
            }
            return match;
        }
        // поиск вертикальных линий из заданной точки   
        public List<MatchBase> GetMatchVert(int col, int row)
        {
            List<MatchBase> match = new List<MatchBase>();
            for (int i = 0; row + i < _row; i++)
            {
                if (allMathElements[col, row].MyColor == allMathElements[col, row + i].MyColor)
                    match.Add(new MatchBase(col, row + i, allMathElements[col, row + i].MyColor));
                else
                    return match;
            }
            return match;
        }

        // На поле не должно быть изначально нерешаемой композиции.
        public bool CheckForPossibles()
        {
            for (int col = 0; col < _col; col++)
            {
                for (int row = 0; row < _row; row++)
                {
                    // воможна горизонтальная, две подряд      
                    if (MatchPattern(col,
                                     row,
                                     new int[,] { { 1, 0 } },
                                     new int[,] { { -2, 0 }, { -1, -1 }, { -1, 1 }, { 2, -1 }, { 2, 1 }, { 3, 0 } }))
                        return true;

                    //// воможна горизонтальная, две по разным сторонам      
                    if (MatchPattern(col,
                                     row,
                                     new int[,] { { 2, 0 } },
                                     new int[,] { { 1, -1 }, { 1, 1 } }))
                        return true;

                    // возможна вертикальная, две подряд      
                    if (MatchPattern(col,
                                     row,
                                     new int[,] { { 0, 1 } },
                                     new int[,] { { 0, -2 }, { -1, -1 }, { 1, -1 }, { -1, 2 }, { 1, 2 }, { 0, 3 } }))
                        return true;

                    // воможна вертикальная, две по разным сторонам       
                    if (MatchPattern(col,
                                     row,
                                     new int[,] { { 0, 2 } },
                                     new int[,] { { -1, 1 }, { 1, 1 } }))
                        return true;
                }
            }
            // не найдено возможных линий    
            return false;
        }

        // Если хотя бы одна фишка совпадает по типу в вказаном диапазоне mustHave или needOne, возвращаем true. Если ни одна, возвращаем false.
        public bool MatchPattern(int col, int row, int[,] mustHave, int[,] needOne)
        {
            // какой тип проверяемой ячейки.
            Color thisColor = allMathElements[col, row].MyColor;

            // убедимся, что есть вторая фишка одного типа    
            for (int i = 0; i < mustHave.GetLength(0); i++)
            {
                if (!MatchType(col + mustHave[i, 0], row + mustHave[i, 1], thisColor))
                    return false;
            }
            // убедимся,  что третья фишка совпадает по типу с двумя другими    
            for (int i = 0; i < needOne.GetLength(0); i++)
            {
                if (MatchType(col + needOne[i, 0], row + needOne[i, 1], thisColor))
                    return true;
            }
            return false;
        }

        public bool MatchType(int col, int row, Color color)
        {
            // убедимся, что фишка не выходит за пределы поля    
            if ((col < 0) || (col >= _col) ||
                (row < 0) || (row >= _row))
                return false;
            return (allMathElements[col, row].MyColor == color);
        }

        //обмен двух фишек   
        public bool MakeSwap(int fromX, int fromY, int toX, int toY)
        {
            SwapPieces(allMathElements[fromX, fromY], allMathElements[toX, toY]);
            // проверяем, был ли обмен удачным    
            if (ChackForMatches().Count > 0)
                return true;
            else
                SwapPieces(allMathElements[fromX, fromY], allMathElements[toX, toY]);
            return false;
        }

        public void SwapPieces(MatchBase a, MatchBase b)
        {
            // обмениваем значения A и B
            MatchBase tmp = new MatchBase(a.X, a.Y, a.MyColor);
            a.X = b.X;
            a.Y = b.Y;
            b.X = tmp.X;
            b.Y = tmp.Y;
            allMathElements[a.X, a.Y] = a;
            allMathElements[b.X, b.Y] = b;
        }

        public void FindAndRemoveMatches()
        {
            List<MatchBase> RemoveElements = new List<MatchBase>();
            foreach (List<MatchBase> MatchLine in ChackForMatches())
            {
                //Количество очков зависит от количества фишек в линии. 
                //Три фишки означают (3-1)*50 или 100 очков за каждую фишку.
                //Четыре фишки, (4-1)*50 или 150 очков за фишку, минимум 600 очков.
                if (OnUpdateScoreEvent != null)
                {
                    int score = (MatchLine.Count - 1) * (MatchLine.Count * 50);
                    OnUpdateScoreEvent(score);
                }
                foreach (MatchBase element in MatchLine)
                {
                    RemoveElements.Add(element);
                    allMathElements[element.X, element.Y] = null;
                }
                foreach (MatchBase element in MatchLine)
                {
                    affectAbove(element.X, element.Y);
                }
            }
            if (OnRemoveMatchesEvent != null)
                OnRemoveMatchesEvent(RemoveElements);
        }


        //заставляет все фишки(их значения) над переданной в функцию двигаться вниз   
        public void affectAbove(int x, int y)
        {
            for (int i = 0; i < _row-1; i++)
            {
                if (allMathElements[x, i] == null && allMathElements[x, i + 1] != null)
                {
                    allMathElements[x, i + 1].Y--;
                    allMathElements[x, i] = allMathElements[x, i + 1];
                    allMathElements[x, i + 1] = null;
                }
            }
        }

        // если в колонке отсутствует фишка, добавляем новую, падающую сверху.   
        public void AddNewPieces()
        {
            List<MatchBase> newElements = new List<MatchBase>();
            for (int x = 0; x < _col; x++)
            {
                for (int y = 0; y < _row; y++)
                {
                    if (allMathElements[x, y] == null)
                    {
                        int randomLength = (_col + _row > 10) ? GameColors.Length : GameColors.Length / 2;
                        int colorIndex = UnityEngine.Random.Range(0, randomLength);
                        allMathElements[x, y] = new MatchBase(x, y, GameColors[colorIndex]);
                        newElements.Add(allMathElements[x, y]);
                    }
                }
            }
            if (OnCreateNewEvent != null)
                OnCreateNewEvent(newElements);
        }
        public void ReadyToCheck()
        {
            // готових линий не найдено
            if (ChackForMatches().Count == 0)
            {
                //проверим на возможность хода
                if (!CheckForPossibles())
                {
                    if (OnGameOverEvent != null)
                        OnGameOverEvent();
                }
                else
                {
                    if (OnContinueGamePlayEvent != null)
                        OnContinueGamePlayEvent();
                }
            }
            else
            {
                FindAndRemoveMatches();
            }
        }
    }
}