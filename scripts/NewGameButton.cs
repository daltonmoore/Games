using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SA
{
    public class NewGameButton : MonoBehaviour
    {
        public Button start;

        private void Start()
        {
            start = GetComponent<Button>();
            start.onClick.AddListener(TaskOnClick);
        }

        void TaskOnClick()
        {
            SceneManager.LoadScene("scene1");
        }
    }
}