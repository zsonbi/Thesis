using NUnit.Framework;
using System.Collections;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;
using System.Reflection;
using System.Linq;
using System;
using Config;

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

            private IEnumerator CreateTask(string taskName, string description, bool taskType, int periodIndex)
            {
                int prevTaskCount = MainController.Tasks.Count;

                taskOpenPanelController.OpenUp();
                if (taskType)
                {
                    taskOpenPanelController.MakeItBadHabit();
                }
                else
                {
                    taskOpenPanelController.MakeItGoodTask();
                }
                TaskName.text = taskName;
                TaskDescription.text = description;
                TaskIntervals.value = periodIndex;
                taskOpenPanelController.Save();
                for (int j = 0; j < 300; j++)
                {
                    if (prevTaskCount != MainController.Tasks.Count)
                    {
                        break;
                    }
                    yield return new WaitForSeconds(0.1f);
                }

                TaskDisplayHandler created = MainController.Tasks.Last().Value;

                Assert.AreEqual(taskName, created.CurrentTask.TaskName);
                Assert.AreEqual(description, created.CurrentTask.Description);
                Assert.AreEqual(TaskOpenPanelController.TASKINTERVALS[periodIndex], created.CurrentTask.PeriodRate);
                Assert.AreEqual(taskType, created.CurrentTask.TaskType);
                Assert.AreNotEqual(-1, MainController.Tasks.Last().Key);
                Assert.True((TimeSpan.FromMinutes(created.CurrentTask.PeriodRate) - (DateTime.UtcNow - created.CurrentTask.LastCompleted)).TotalSeconds <= 0);

                yield return null;
            }

            [UnityTest]
            public IEnumerator CreateGoodTasksTest()
            {
                yield return LoadScene();

                //Add money and task score
                CoroutineRunner.RunCoroutine(Server.SendPostRequest<Thesis_backend.Data_Structures.User>(TestConfig.TASK_SCORE_CHEAT_PATH, TestConfig.TASK_SCORE_CHEAT_PASSWORD));
                CoroutineRunner.RunCoroutine(Server.SendPatchRequest<Thesis_backend.Data_Structures.Game>(ServerConfig.PATH_FOR_SAVE_COINS, 10000));
                yield return new WaitForSeconds(TestConfig.ANSWER_TOLERANCE);

                taskOpenPanelController.OpenUp();
                MainController.LoadGoodTasks();
                yield return new WaitForSeconds(TestConfig.ANSWER_TOLERANCE);

                LoadTaskComponents();
                int initCount = TaskParent.transform.childCount;
                long uniqueId = DateTime.Now.Ticks;
                //Make the good habits
                for (int i = 0; i < TaskIntervals.options.Count; i++)
                {
                    yield return CreateTask("test" + uniqueId + i, "testDescription" + uniqueId + i, false, i);
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
                yield return new WaitForSeconds(TestConfig.ANSWER_TOLERANCE);

                taskOpenPanelController.OpenUp();

                LoadTaskComponents();
                int initCount = TaskParent.transform.childCount;
                long uniqueId = DateTime.Now.Ticks;

                for (int i = 0; i < TaskIntervals.options.Count; i++)
                {
                    yield return CreateTask("test" + uniqueId + i, "testDescription" + uniqueId + i, true, i);
                }

                MainController.LoadBadHabits();
                yield return new WaitForSeconds(TestConfig.ANSWER_TOLERANCE);
                Assert.AreEqual(TaskIntervals.options.Count + initCount, TaskParent.transform.childCount);
            }

            [UnityTest]
            public IEnumerator TaskCancelTest()
            {
                yield return LoadScene();
                MainController.LoadGoodTasks();
                yield return new WaitForSeconds(TestConfig.ANSWER_TOLERANCE);
                taskOpenPanelController.OpenUp();
                LoadTaskComponents();
                long uniqueId = DateTime.Now.Ticks;

                yield return CreateTask("testCancel" + uniqueId, "testDescription" + uniqueId, true, 0);

                taskOpenPanelController.OpenUp(MainController.Tasks.Last().Value.CurrentTask, TaskType.BadHabit);
                taskOpenPanelController.MakeItBadHabit();
                TaskName.text = "testCancel2" + uniqueId;
                TaskDescription.text = "testDescriptiondwaadw" + uniqueId;
                TaskIntervals.value = 4;
                taskOpenPanelController.Cancel();

                yield return new WaitForSeconds(TestConfig.ANSWER_TOLERANCE);

                Assert.AreEqual("testCancel" + uniqueId, MainController.Tasks.Last().Value.CurrentTask.TaskName);
                Assert.AreEqual("testDescription" + uniqueId, MainController.Tasks.Last().Value.CurrentTask.Description);
                Assert.AreEqual(TaskOpenPanelController.TASKINTERVALS[0], MainController.Tasks.Last().Value.CurrentTask.PeriodRate);
                Assert.AreEqual(true, MainController.Tasks.Last().Value.CurrentTask.TaskType);
                Assert.AreNotEqual(-1, MainController.Tasks.Last().Key);
                Assert.True((TimeSpan.FromMinutes(MainController.Tasks.Last().Value.CurrentTask.PeriodRate) - (DateTime.UtcNow - MainController.Tasks.Last().Value.CurrentTask.LastCompleted)).TotalSeconds <= 0);
            }

            [UnityTest]
            public IEnumerator TaskDeleteTest()
            {
                yield return LoadScene();
                MainController.LoadGoodTasks();
                yield return new WaitForSeconds(TestConfig.ANSWER_TOLERANCE);
                taskOpenPanelController.OpenUp();
                LoadTaskComponents();

                long uniqueId = DateTime.Now.Ticks;
                int prevTaskCount = MainController.Tasks.Count;

                yield return CreateTask("testDelete" + uniqueId, "testDescription" + uniqueId, true, 0);

                taskOpenPanelController.OpenUp(MainController.Tasks.Last().Value.CurrentTask, TaskType.BadHabit);
                taskOpenPanelController.DeleteTask();

                for (int j = 0; j < 300; j++)
                {
                    if (prevTaskCount == MainController.Tasks.Count)
                    {
                        break;
                    }
                    yield return new WaitForSeconds(0.1f);
                }

                Assert.AreEqual(prevTaskCount, MainController.Tasks.Count);
            }

            [UnityTest]
            public IEnumerator TaskSaveTest1()
            {
                yield return LoadScene();
                MainController.LoadGoodTasks();
                yield return new WaitForSeconds(TestConfig.ANSWER_TOLERANCE);
                taskOpenPanelController.OpenUp();
                LoadTaskComponents();
                long uniqueId = DateTime.Now.Ticks;

                yield return CreateTask("testSave1" + uniqueId, "testDescription" + uniqueId, true, 0);

                MainController.LoadBadHabits();

                taskOpenPanelController.OpenUp(MainController.Tasks.Last().Value.CurrentTask, TaskType.BadHabit);
                taskOpenPanelController.MakeItGoodTask();
                TaskName.text = "testSaveMod1" + uniqueId;
                TaskDescription.text = "testDescriptionMod" + uniqueId;
                TaskIntervals.value = 4;
                taskOpenPanelController.Save();

                yield return new WaitForSeconds(TestConfig.ANSWER_TOLERANCE);

                Assert.AreEqual("testSaveMod1" + uniqueId, MainController.Tasks.Last().Value.CurrentTask.TaskName);
                Assert.AreEqual("testDescriptionMod" + uniqueId, MainController.Tasks.Last().Value.CurrentTask.Description);
                Assert.AreEqual(TaskOpenPanelController.TASKINTERVALS[4], MainController.Tasks.Last().Value.CurrentTask.PeriodRate);
                Assert.AreEqual(false, MainController.Tasks.Last().Value.CurrentTask.TaskType);
                Assert.AreNotEqual(-1, MainController.Tasks.Last().Key);
                Assert.True((TimeSpan.FromMinutes(MainController.Tasks.Last().Value.CurrentTask.PeriodRate) - (DateTime.UtcNow - MainController.Tasks.Last().Value.CurrentTask.LastCompleted)).TotalSeconds <= 0);
            }

            [UnityTest]
            public IEnumerator TaskSaveTest2()
            {
                yield return LoadScene();
                MainController.LoadGoodTasks();
                yield return new WaitForSeconds(TestConfig.ANSWER_TOLERANCE);
                taskOpenPanelController.OpenUp();
                LoadTaskComponents();
                long uniqueId = DateTime.Now.Ticks;

                yield return CreateTask("testSave2" + uniqueId, "testDescription" + uniqueId, false, 0);

                MainController.LoadBadHabits();

                taskOpenPanelController.OpenUp(MainController.Tasks.Last().Value.CurrentTask, TaskType.BadHabit);
                taskOpenPanelController.MakeItBadHabit();
                TaskName.text = "testSaveMod2" + uniqueId;
                TaskDescription.text = "testDescriptionMod" + uniqueId;
                TaskIntervals.value = 2;
                taskOpenPanelController.Save();

                yield return new WaitForSeconds(TestConfig.ANSWER_TOLERANCE);

                Assert.AreEqual("testSaveMod2" + uniqueId, MainController.Tasks.Last().Value.CurrentTask.TaskName);
                Assert.AreEqual("testDescriptionMod" + uniqueId, MainController.Tasks.Last().Value.CurrentTask.Description);
                Assert.AreEqual(TaskOpenPanelController.TASKINTERVALS[2], MainController.Tasks.Last().Value.CurrentTask.PeriodRate);
                Assert.AreEqual(true, MainController.Tasks.Last().Value.CurrentTask.TaskType);
                Assert.AreNotEqual(-1, MainController.Tasks.Last().Key);
                Assert.True((TimeSpan.FromMinutes(MainController.Tasks.Last().Value.CurrentTask.PeriodRate) - (DateTime.UtcNow - MainController.Tasks.Last().Value.CurrentTask.LastCompleted)).TotalSeconds <= 0);
            }

            [UnityTest]
            public IEnumerator TestTaskComplete()
            {
                yield return LoadScene();
                MainController.LoadGoodTasks();
                yield return new WaitForSeconds(TestConfig.ANSWER_TOLERANCE);
                taskOpenPanelController.OpenUp();
                LoadTaskComponents();
                long uniqueId = DateTime.Now.Ticks;

                yield return CreateTask("testComplete" + uniqueId, "testDescription" + uniqueId, false, 0);

                MainController.Tasks.Last().Value.CompleteTask();
                yield return new WaitForSeconds(TestConfig.ANSWER_TOLERANCE);

                Assert.True((TimeSpan.FromMinutes(MainController.Tasks.Last().Value.CurrentTask.PeriodRate) - (DateTime.UtcNow - MainController.Tasks.Last().Value.CurrentTask.LastCompleted)).TotalSeconds > 600);
            }
        }
    }
}