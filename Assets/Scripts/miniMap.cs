using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Color = UnityEngine.Color;

namespace Dune2 {
    public class miniMap : MonoBehaviour {
        private int width = 256;
        private int height = 256;
        private RawImage miniMapUI;
        private RectTransform cameraFrame;
        private Camera mainCamera;
        private RectTransform miniMapRect;
        
        void Start() {
            miniMapUI = GetComponent<RawImage>();
            miniMapRect = miniMapUI.GetComponent<RectTransform>();
            mainCamera = Camera.main;
            //width = miniMapUI.rectTransform.
            CreateCameraFrame();
        }

        void Update() {
            UpdateCameraFrame();
        }

        public void Activate()
        {
            if (miniMapUI != null)
                miniMapUI.enabled = true;
        }

        public void Deactivate()
        {
            if (miniMapUI != null)
                miniMapUI.enabled = false;
        }

        public void UpdateData()
        {
        }
        
        private void CreateCameraFrame()
        {
            // Создание GameObject для рамки камеры
            GameObject frameObject = new GameObject("CameraFrame");
            frameObject.transform.SetParent(miniMapUI.transform, false);
            
            // Добавление компонента Image
            UnityEngine.UI.Image frameImage = frameObject.AddComponent<UnityEngine.UI.Image>();
            frameImage.color = new Color(1, 1, 1, 0.3f); // Полупрозрачный белый
            
            // Добавление Outline для видимости границ
            Outline outline = frameObject.AddComponent<Outline>();
            outline.effectColor = new Color(1, 0.85f, 0, 1); // Жёлтый контур
            outline.effectDistance = new Vector2(2, 2);
            
            // Получение RectTransform
            cameraFrame = frameObject.GetComponent<RectTransform>();
            cameraFrame.anchorMin = new Vector2(0, 1); // Верхний левый угол
            cameraFrame.anchorMax = new Vector2(0, 1);
            cameraFrame.pivot = new Vector2(0, 1);
        }
        
        private void UpdateCameraFrame()
        {
            if (mainCamera == null || cameraFrame == null || miniMapRect == null)
                return;
                
            Vector2Int mapSize = gameManager.GetInstance().GetMapSize();
            Vector2 cellSize = gameManager.GetInstance().GetCellSize();

            Debug.Log($"MapSize: {mapSize}, CellSize: {cellSize}"); // <-- ДОБАВЬТЕ ЭТУ СТРОКУ
            
            if (mapSize.x == 0 || mapSize.y == 0 || cellSize.x == 0 || cellSize.y == 0)
                return;


            // Размеры карты в мировых координатах
            float mapWidthWorld = mapSize.x * cellSize.x;
            float mapHeightWorld = mapSize.y * cellSize.y;

            // Границы видимой области камеры в мировых координатах
            float orthoSize = mainCamera.orthographicSize;
            float aspectRatio = (float)Screen.width / Screen.height;
            Vector3 cameraPos = mainCamera.transform.position;

            float cameraLeft = cameraPos.x - orthoSize * aspectRatio;
            float cameraRight = cameraPos.x + orthoSize * aspectRatio;
            float cameraBottom = cameraPos.y - orthoSize;
            float cameraTop = cameraPos.y + orthoSize;

            // Нормализуем координаты границ камеры (0-1)
            float normalizedLeft = cameraLeft / mapWidthWorld;
            float normalizedRight = cameraRight / mapWidthWorld;
            float normalizedBottom = cameraBottom / mapHeightWorld;
            float normalizedTop = cameraTop / mapHeightWorld;

            // Преобразуем в пиксели миникарты
            float pixelLeft = normalizedLeft * width;
            float pixelRight = normalizedRight * width;
            float pixelBottom = normalizedBottom * height;
            float pixelTop = normalizedTop * height;
            // Нормализуем позицию центра камеры (0-1)
            float normalizedX = cameraPos.x / mapWidthWorld;
            float normalizedY = cameraPos.y / mapHeightWorld;
            float pixelX = normalizedX * width;
            float pixelY = normalizedY * height;

            // Рассчитываем размер и позицию рамки
            float frameWidth = pixelRight - pixelLeft;
            float frameHeight = pixelTop - pixelBottom;

            cameraFrame.sizeDelta = new Vector2(frameWidth* cellSize.x, frameHeight* cellSize.y);
            cameraFrame.anchoredPosition = new Vector2(width*pixelLeft*0.5f/orthoSize, height*pixelTop*0.5f/orthoSize);
        }
    }
}
