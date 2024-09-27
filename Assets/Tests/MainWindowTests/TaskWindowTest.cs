using Assets.Tests;
using Assets.Tests.MainWindowTests.MainWindowTests;
using NUnit.Framework;
using System.Collections;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;
using System.Reflection;

namespace Tests
{
    namespace MainWindowTests
    {
        public class TaskWindowTest : MainWindowTestsParent<MainWindowController>
        {
            private Button GoodTaskPanelButton;
            private Button BadTaskPanelButton;
            private Button CancelTaskPanelButton;
            private Button SaveTaskPanelButton;
            private Button CloseTaskPanelButton;
            private Button DeleteTaskPanelButton;
            private TMP_InputField TaskName;
            private TMP_InputField TaskDescription;
            private TMP_Dropdown TaskIntervals;
            private GameObject TaskParent;

            private void LoadTaskComponents()
            {
                GoodTaskPanelButton = GameObject.Find(TestConfig.TASK_PANEL_GOOD_TASK_BUTTON_NAME).GetComponent<Button>();
                BadTaskPanelButton = GameObject.Find(TestConfig.TASK_PANEL_BAD_TASK_BUTTON_NAME).GetComponent<Button>();
                CancelTaskPanelButton = GameObject.Find(TestConfig.TASK_PANEL_CANCEL_TASK_BUTTON_NAME).GetComponent<Button>();
                SaveTaskPanelButton = GameObject.Find(TestConfig.TASK_PANEL_SAVE_TASK_BUTTON_NAME).GetComponent<Button>();
                CloseTaskPanelButton = GameObject.Find(TestConfig.TASK_PANEL_CLOSE_BUTTON_NAME).GetComponent<Button>();

                TaskName = GameObject.Find(TestConfig.TASK_PANEL_TASK_NAME_INPUT_NAME).GetComponent<TMP_InputField>();
                TaskDescription = GameObject.Find(TestConfig.TASK_PANEL_TASK_NAME_DESCRIPTION_NAME).GetComponent<TMP_InputField>();

                TaskIntervals = GameObject.Find(TestConfig.TASK_PANEL_TASK_PERIOD_DROPDOWN_NAME).GetComponent<TMP_Dropdown>();

                TaskParent = GameObject.Find(TestConfig.TASK_PARENT_NAME);
            }

            [UnityTest]
            public IEnumerator CreateGoodTasksTest()
            {
                yield return LoadScene();

                taskOpenPanelController.OpenUp();
                MainController.LoadGoodTasks();

                LoadTaskComponents();
                int initCount = TaskParent.transform.childCount;
                //Make the good habits
                for (int i = 0; i < TaskIntervals.options.Count; i++)
                {
                    taskOpenPanelController.OpenUp();
                    taskOpenPanelController.MakeItGoodTask();
                    TaskName.text = "testGood" + i;
                    TaskDescription.text = "testGoodDescription" + i;
                    TaskIntervals.value = i;
                    taskOpenPanelController.Save();
                    yield return new WaitForSeconds(TestConfig.ANSWER_TOLERANCE / 3f);
                }
                MainController.LoadGoodTasks();
                yield return new WaitForSeconds(TestConfig.ANSWER_TOLERANCE);
                Assert.AreEqual(TaskIntervals.options.Count + initCount, TaskParent.transform.childCount);
            }

            [UnityTest]
            public IEnumerator CreateBadTasksTest()
            {
                yield return LoadScene();
                MainController.LoadBadHabits();

                taskOpenPanelController.OpenUp();

                LoadTaskComponents();
                int initCount = TaskParent.transform.childCount;
                //Make the good habits
                for (int i = 0; i < TaskIntervals.options.Count; i++)
                {
                    taskOpenPanelController.OpenUp();
                    taskOpenPanelController.MakeItBadHabit();
                    TaskName.text = "testBad" + i;
                    TaskDescription.text = "testBadDescription" + i;
                    TaskIntervals.value = i;
                    taskOpenPanelController.Save();
                    yield return new WaitForSeconds(TestConfig.ANSWER_TOLERANCE / 3f);
                }
                MainController.LoadBadHabits();
                yield return new WaitForSeconds(TestConfig.ANSWER_TOLERANCE);
                Assert.AreEqual(TaskIntervals.options.Count + initCount, TaskParent.transform.childCount);
            }
        }
    }
}