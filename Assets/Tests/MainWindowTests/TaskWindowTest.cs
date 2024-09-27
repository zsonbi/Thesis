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
using System.Linq;
using System;
using static UnityEditor.Progress;

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
                yield return new WaitForSeconds(TestConfig.ANSWER_TOLERANCE);

                LoadTaskComponents();
                int initCount = TaskParent.transform.childCount;
                int taskOffset = MainController.Tasks.Count;
                long uniqueId = DateTime.Now.Ticks;
                //Make the good habits
                for (int i = 0; i < TaskIntervals.options.Count; i++)
                {
                    int prevTaskCount = MainController.Tasks.Count;

                    taskOpenPanelController.OpenUp();
                    taskOpenPanelController.MakeItGoodTask();
                    TaskName.text = "test" + uniqueId + i;
                    TaskDescription.text = "testDescription" + uniqueId + i;
                    TaskIntervals.value = i;
                    taskOpenPanelController.Save();

                    for (int j = 0; j < 300; j++)
                    {
                        if (prevTaskCount != MainController.Tasks.Count)
                        {
                            break;
                        }
                        yield return new WaitForSeconds(0.1f);
                    }

                    //  yield return new WaitForSeconds(TestConfig.ANSWER_TOLERANCE);
                }
                MainController.LoadGoodTasks();
                yield return new WaitForSeconds(TestConfig.ANSWER_TOLERANCE);
                Assert.AreEqual(TaskIntervals.options.Count + initCount, TaskParent.transform.childCount);

                int counter = 0;
                foreach (var item in MainController.Tasks.Skip(taskOffset))
                {
                    Assert.AreEqual("test" + uniqueId + counter, item.Value.CurrentTask.TaskName);
                    Assert.AreEqual("testDescription" + uniqueId + counter, item.Value.CurrentTask.Description);
                    Assert.AreEqual(TaskOpenPanelController.TASKINTERVALS[counter], item.Value.CurrentTask.PeriodRate);
                    Assert.AreEqual(false, item.Value.CurrentTask.TaskType);
                    Assert.AreNotEqual(-1, item.Key);
                    Assert.True((TimeSpan.FromMinutes(item.Value.CurrentTask.PeriodRate) - (DateTime.UtcNow - item.Value.CurrentTask.LastCompleted)).TotalSeconds <= 0);
                    counter++;
                }
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
                int taskOffset = MainController.Tasks.Count;

                //Make the good habits
                for (int i = 0; i < TaskIntervals.options.Count; i++)
                {
                    int prevTaskCount = MainController.Tasks.Count;

                    taskOpenPanelController.OpenUp();
                    taskOpenPanelController.MakeItBadHabit();
                    TaskName.text = "test" + uniqueId + i;
                    TaskDescription.text = "testDescription" + uniqueId + i;
                    TaskIntervals.value = i;
                    taskOpenPanelController.Save();

                    for (int j = 0; j < 300; j++)
                    {
                        if (prevTaskCount != MainController.Tasks.Count)
                        {
                            break;
                        }
                        yield return new WaitForSeconds(0.1f);
                    }

                    //  yield return new WaitForSeconds(TestConfig.ANSWER_TOLERANCE);
                }
                MainController.LoadBadHabits();
                yield return new WaitForSeconds(TestConfig.ANSWER_TOLERANCE);
                Assert.AreEqual(TaskIntervals.options.Count + initCount, TaskParent.transform.childCount);

                int counter = 0;
                foreach (var item in MainController.Tasks.Skip(taskOffset))
                {
                    Assert.AreEqual("test" + uniqueId + counter, item.Value.CurrentTask.TaskName);
                    Assert.AreEqual("testDescription" + uniqueId + counter, item.Value.CurrentTask.Description);
                    Assert.AreEqual(TaskOpenPanelController.TASKINTERVALS[counter], item.Value.CurrentTask.PeriodRate);
                    Assert.AreEqual(true, item.Value.CurrentTask.TaskType);
                    Assert.AreNotEqual(-1, item.Key);
                    Assert.True((TimeSpan.FromMinutes(item.Value.CurrentTask.PeriodRate) - (DateTime.UtcNow - item.Value.CurrentTask.LastCompleted)).TotalSeconds <= 0);

                    counter++;
                }
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
                int prevTaskCount = MainController.Tasks.Count;
                taskOpenPanelController.OpenUp();
                taskOpenPanelController.MakeItBadHabit();
                TaskName.text = "testCancel" + uniqueId;
                TaskDescription.text = "testDescription" + uniqueId;
                TaskIntervals.value = 0;
                taskOpenPanelController.Save();

                for (int j = 0; j < 300; j++)
                {
                    if (prevTaskCount != MainController.Tasks.Count)
                    {
                        break;
                    }
                    yield return new WaitForSeconds(0.1f);
                }

                MainController.LoadBadHabits();

                yield return new WaitForSeconds(TestConfig.ANSWER_TOLERANCE);
                Assert.AreEqual("testCancel" + uniqueId, MainController.Tasks.Last().Value.CurrentTask.TaskName);
                Assert.AreEqual("testDescription" + uniqueId, MainController.Tasks.Last().Value.CurrentTask.Description);
                Assert.AreEqual(TaskOpenPanelController.TASKINTERVALS[0], MainController.Tasks.Last().Value.CurrentTask.PeriodRate);
                Assert.AreEqual(true, MainController.Tasks.Last().Value.CurrentTask.TaskType);
                Assert.AreNotEqual(-1, MainController.Tasks.Last().Key);
                Assert.True((TimeSpan.FromMinutes(MainController.Tasks.Last().Value.CurrentTask.PeriodRate) - (DateTime.UtcNow - MainController.Tasks.Last().Value.CurrentTask.LastCompleted)).TotalSeconds <= 0);

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
                taskOpenPanelController.OpenUp();
                taskOpenPanelController.MakeItBadHabit();
                TaskName.text = "testDelete" + uniqueId;
                TaskDescription.text = "testDescription" + uniqueId;
                TaskIntervals.value = 0;
                taskOpenPanelController.Save();

                for (int j = 0; j < 300; j++)
                {
                    if (prevTaskCount != MainController.Tasks.Count)
                    {
                        break;
                    }
                    yield return new WaitForSeconds(0.1f);
                }

                MainController.LoadBadHabits();

                yield return new WaitForSeconds(TestConfig.ANSWER_TOLERANCE);
                Assert.AreEqual("testDelete" + uniqueId, MainController.Tasks.Last().Value.CurrentTask.TaskName);
                Assert.AreEqual("testDescription" + uniqueId, MainController.Tasks.Last().Value.CurrentTask.Description);
                Assert.AreEqual(TaskOpenPanelController.TASKINTERVALS[0], MainController.Tasks.Last().Value.CurrentTask.PeriodRate);
                Assert.AreEqual(true, MainController.Tasks.Last().Value.CurrentTask.TaskType);
                Assert.AreNotEqual(-1, MainController.Tasks.Last().Key);
                Assert.True((TimeSpan.FromMinutes(MainController.Tasks.Last().Value.CurrentTask.PeriodRate) - (DateTime.UtcNow - MainController.Tasks.Last().Value.CurrentTask.LastCompleted)).TotalSeconds <= 0);

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
                int prevTaskCount = MainController.Tasks.Count;
                taskOpenPanelController.OpenUp();
                taskOpenPanelController.MakeItBadHabit();
                TaskName.text = "testSave1" + uniqueId;
                TaskDescription.text = "testDescription" + uniqueId;
                TaskIntervals.value = 0;
                taskOpenPanelController.Save();

                for (int j = 0; j < 300; j++)
                {
                    if (prevTaskCount != MainController.Tasks.Count)
                    {
                        break;
                    }
                    yield return new WaitForSeconds(0.1f);
                }

                MainController.LoadBadHabits();

                yield return new WaitForSeconds(TestConfig.ANSWER_TOLERANCE);
                Assert.AreEqual("testSave1" + uniqueId, MainController.Tasks.Last().Value.CurrentTask.TaskName);
                Assert.AreEqual("testDescription" + uniqueId, MainController.Tasks.Last().Value.CurrentTask.Description);
                Assert.AreEqual(TaskOpenPanelController.TASKINTERVALS[0], MainController.Tasks.Last().Value.CurrentTask.PeriodRate);
                Assert.AreEqual(true, MainController.Tasks.Last().Value.CurrentTask.TaskType);
                Assert.AreNotEqual(-1, MainController.Tasks.Last().Key);
                Assert.True((TimeSpan.FromMinutes(MainController.Tasks.Last().Value.CurrentTask.PeriodRate) - (DateTime.UtcNow - MainController.Tasks.Last().Value.CurrentTask.LastCompleted)).TotalSeconds <= 0);

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
                int prevTaskCount = MainController.Tasks.Count;
                taskOpenPanelController.OpenUp();
                taskOpenPanelController.MakeItGoodTask();
                TaskName.text = "testSave2" + uniqueId;
                TaskDescription.text = "testDescription" + uniqueId;
                TaskIntervals.value = 0;
                taskOpenPanelController.Save();

                for (int j = 0; j < 300; j++)
                {
                    if (prevTaskCount != MainController.Tasks.Count)
                    {
                        break;
                    }
                    yield return new WaitForSeconds(0.1f);
                }

                MainController.LoadBadHabits();

                yield return new WaitForSeconds(TestConfig.ANSWER_TOLERANCE);
                Assert.AreEqual("testSave2" + uniqueId, MainController.Tasks.Last().Value.CurrentTask.TaskName);
                Assert.AreEqual("testDescription" + uniqueId, MainController.Tasks.Last().Value.CurrentTask.Description);
                Assert.AreEqual(TaskOpenPanelController.TASKINTERVALS[0], MainController.Tasks.Last().Value.CurrentTask.PeriodRate);
                Assert.AreEqual(false, MainController.Tasks.Last().Value.CurrentTask.TaskType);
                Assert.AreNotEqual(-1, MainController.Tasks.Last().Key);
                Assert.True((TimeSpan.FromMinutes(MainController.Tasks.Last().Value.CurrentTask.PeriodRate) - (DateTime.UtcNow - MainController.Tasks.Last().Value.CurrentTask.LastCompleted)).TotalSeconds <= 0);

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
                int prevTaskCount = MainController.Tasks.Count;
                taskOpenPanelController.OpenUp();
                taskOpenPanelController.MakeItGoodTask();
                TaskName.text = "testComplete" + uniqueId;
                TaskDescription.text = "testDescription" + uniqueId;
                TaskIntervals.value = 0;
                taskOpenPanelController.Save();

                for (int j = 0; j < 300; j++)
                {
                    if (prevTaskCount != MainController.Tasks.Count)
                    {
                        break;
                    }
                    yield return new WaitForSeconds(0.1f);
                }

                yield return new WaitForSeconds(TestConfig.ANSWER_TOLERANCE);
                Assert.AreEqual("testComplete" + uniqueId, MainController.Tasks.Last().Value.CurrentTask.TaskName);
                Assert.AreEqual("testDescription" + uniqueId, MainController.Tasks.Last().Value.CurrentTask.Description);
                Assert.AreEqual(TaskOpenPanelController.TASKINTERVALS[0], MainController.Tasks.Last().Value.CurrentTask.PeriodRate);
                Assert.AreEqual(false, MainController.Tasks.Last().Value.CurrentTask.TaskType);
                Assert.AreNotEqual(-1, MainController.Tasks.Last().Key);
                Assert.True((TimeSpan.FromMinutes(MainController.Tasks.Last().Value.CurrentTask.PeriodRate) - (DateTime.UtcNow - MainController.Tasks.Last().Value.CurrentTask.LastCompleted)).TotalSeconds <= 0);

                MainController.Tasks.Last().Value.CompleteTask();
                yield return new WaitForSeconds(TestConfig.ANSWER_TOLERANCE);

                Assert.True((TimeSpan.FromMinutes(MainController.Tasks.Last().Value.CurrentTask.PeriodRate) - (DateTime.UtcNow - MainController.Tasks.Last().Value.CurrentTask.LastCompleted)).TotalSeconds > 600);
            }
        }
    }
}