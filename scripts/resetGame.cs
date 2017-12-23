using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SA
{
    public class resetGame : MonoBehaviour
    {
        public Button start;
        public Button exit;
        public StateManager states;

        private void Start()
        {
            start = GetComponent<Button>();
            start.onClick.AddListener(TaskOnClick);
            exit.onClick.AddListener(Exit);
        }

        void Exit()
        {
            if (states.paused)
            {
                Application.Quit();
            }
        }

        void TaskOnClick()
        {
            if (states.paused)
            {
                SceneManager.LoadScene("scene1");
                Time.timeScale = 1;
            }
        }
    }
}