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
using MainPage;
using UnityEngine.Rendering;
using Thesis_backend.Data_Structures;

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

                yield return WaitForCondition(() => prevTaskCount != MainController.Tasks.Count);
                yield return WaitForCondition(() => taskOpenPanelController.CurrentTask.ID == -1);
                yield return WaitForCondition(() => MainController.Tasks.Values.FirstOrDefault(x => x.CurrentTask.TaskName == taskName && x.CurrentTask.TaskType == taskType) is not null);
                TaskDisplayHandler created = MainController.Tasks.Values.FirstOrDefault(x => x.CurrentTask.TaskName == taskName && x.CurrentTask.TaskType == taskType);
                Assert.NotNull(created);
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

                TaskDisplayHandler task = MainController.Tasks.Values.FirstOrDefault(x => x.CurrentTask.TaskName == "testCancel" + uniqueId && x.CurrentTask.TaskType == true);

                taskOpenPanelController.OpenUp(task.CurrentTask, TaskType.BadHabit);
                taskOpenPanelController.MakeItBadHabit();
                TaskName.text = "testCancel2" + uniqueId;
                TaskDescription.text = "testDescriptiondwaadw" + uniqueId;
                TaskIntervals.value = 4;
                taskOpenPanelController.Cancel();

                yield return WaitForCondition(() => taskOpenPanelController.CurrentTask.ID == -1);
                yield return WaitForCondition(() => MainController.Tasks.Values.FirstOrDefault(x => x.CurrentTask.TaskName == "testCancel" + uniqueId && x.CurrentTask.TaskType == true) is not null);

                PlayerTask canceledTask = MainController.Tasks.Values.FirstOrDefault(x => x.CurrentTask.TaskName == "testCancel" + uniqueId && x.CurrentTask.TaskType == true).CurrentTask;

                Assert.AreEqual("testCancel" + uniqueId, canceledTask.TaskName);
                Assert.AreEqual("testDescription" + uniqueId, canceledTask.Description);
                Assert.AreEqual(TaskOpenPanelController.TASKINTERVALS[0], canceledTask.PeriodRate);
                Assert.AreEqual(true, canceledTask.TaskType);
                Assert.AreNotEqual(-1, MainController.Tasks.Last().Key);
                Assert.True((TimeSpan.FromMinutes(canceledTask.PeriodRate) - (DateTime.UtcNow - canceledTask.LastCompleted)).TotalSeconds <= 0);
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
                TaskDisplayHandler task = MainController.Tasks.Values.FirstOrDefault(x => x.CurrentTask.TaskName == "testDelete" + uniqueId && x.CurrentTask.TaskType == true);

                taskOpenPanelController.OpenUp(task.CurrentTask, TaskType.BadHabit);
                taskOpenPanelController.DeleteTask();

                yield return WaitForCondition(() => prevTaskCount == MainController.Tasks.Count);
                yield return WaitForCondition(() => MainController.Tasks.Values.FirstOrDefault(x => x.CurrentTask.TaskName == "testCancel" + uniqueId && x.CurrentTask.TaskType == true) is null);

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
                TaskDisplayHandler task = MainController.Tasks.Values.FirstOrDefault(x => x.CurrentTask.TaskName == "testSave1" + uniqueId && x.CurrentTask.TaskType == true);

                taskOpenPanelController.OpenUp(task.CurrentTask, TaskType.BadHabit);
                taskOpenPanelController.MakeItGoodTask();
                TaskName.text = "testSaveMod1" + uniqueId;
                TaskDescription.text = "testDescriptionMod" + uniqueId;
                TaskIntervals.value = 4;
                taskOpenPanelController.Save();
                yield return WaitForCondition(() => taskOpenPanelController.CurrentTask.ID == -1);
                yield return WaitForCondition(() => MainController.Tasks.Values.FirstOrDefault(x => x.CurrentTask.TaskName == "testSaveMod1" + uniqueId && x.CurrentTask.TaskType == false) is not null);
                PlayerTask savedTask = MainController.Tasks.Values.FirstOrDefault(x => x.CurrentTask.TaskName == "testSaveMod1" + uniqueId && x.CurrentTask.TaskType == false).CurrentTask;

                Assert.AreEqual("testSaveMod1" + uniqueId, savedTask.TaskName);
                Assert.AreEqual("testDescriptionMod" + uniqueId, savedTask.Description);
                Assert.AreEqual(TaskOpenPanelController.TASKINTERVALS[4], savedTask.PeriodRate);
                Assert.AreEqual(false, savedTask.TaskType);
                Assert.AreNotEqual(-1, MainController.Tasks.Last().Key);
                Assert.True((TimeSpan.FromMinutes(savedTask.PeriodRate) - (DateTime.UtcNow - savedTask.LastCompleted)).TotalSeconds <= 0);
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
                TaskDisplayHandler task = MainController.Tasks.Values.FirstOrDefault(x => x.CurrentTask.TaskName == "testSave2" + uniqueId && x.CurrentTask.TaskType == false);

                taskOpenPanelController.OpenUp(task.CurrentTask, TaskType.BadHabit);
                taskOpenPanelController.MakeItBadHabit();
                TaskName.text = "testSaveMod2" + uniqueId;
                TaskDescription.text = "testDescriptionMod" + uniqueId;
                TaskIntervals.value = 2;
                taskOpenPanelController.Save();

                yield return WaitForCondition(() => taskOpenPanelController.CurrentTask.ID == -1);
                yield return WaitForCondition(() => MainController.Tasks.Values.FirstOrDefault(x => x.CurrentTask.TaskName == "testSaveMod2" + uniqueId && x.CurrentTask.TaskType == true) is not null);
                PlayerTask savedTask = MainController.Tasks.Values.FirstOrDefault(x => x.CurrentTask.TaskName == "testSaveMod2" + uniqueId && x.CurrentTask.TaskType == true).CurrentTask;

                Assert.AreEqual("testSaveMod2" + uniqueId, savedTask.TaskName);
                Assert.AreEqual("testDescriptionMod" + uniqueId, savedTask.Description);
                Assert.AreEqual(TaskOpenPanelController.TASKINTERVALS[2], savedTask.PeriodRate);
                Assert.AreEqual(true, savedTask.TaskType);
                Assert.AreNotEqual(-1, MainController.Tasks.Last().Key);
                Assert.True((TimeSpan.FromMinutes(savedTask.PeriodRate) - (DateTime.UtcNow - savedTask.LastCompleted)).TotalSeconds <= 0);
            }

            [UnityTest]
            public IEnumerator TestGoodTaskComplete()
            {
                yield return LoadScene();
                MainController.LoadGoodTasks();
                yield return new WaitForSeconds(TestConfig.ANSWER_TOLERANCE);
                taskOpenPanelController.OpenUp();
                LoadTaskComponents();
                long uniqueId = DateTime.Now.Ticks;

                long baseCurrentScore = User.UserData.Instance.CurrentTaskScore;
                long baseTotalScore = User.UserData.Instance.TotalScore;

                yield return CreateTask("testCompleteGood" + uniqueId, "testDescription" + uniqueId, false, 0);
                TaskDisplayHandler task = MainController.Tasks.Values.FirstOrDefault(x => x.CurrentTask.TaskName == "testCompleteGood" + uniqueId && x.CurrentTask.TaskType == false);

                task.CompleteTask();
                yield return WaitForCondition(() => task.CurrentTask.Completed);
                yield return WaitForCondition(() => baseCurrentScore < User.UserData.Instance.CurrentTaskScore);
                Assert.Greater((TimeSpan.FromMinutes(task.CurrentTask.PeriodRate) - (DateTime.UtcNow - task.CurrentTask.LastCompleted)).TotalSeconds, 600);
                Assert.Less(baseCurrentScore, User.UserData.Instance.CurrentTaskScore);
                Assert.Less(baseTotalScore, User.UserData.Instance.TotalScore);
            }

            [UnityTest]
            public IEnumerator TestBadTaskComplete()
            {
                yield return LoadScene();
                MainController.LoadGoodTasks();
                yield return new WaitForSeconds(TestConfig.ANSWER_TOLERANCE);
                taskOpenPanelController.OpenUp();
                LoadTaskComponents();
                long uniqueId = DateTime.Now.Ticks;

                long baseCurrentScore = User.UserData.Instance.CurrentTaskScore;
                long baseTotalScore = User.UserData.Instance.TotalScore;

                yield return CreateTask("testCompleteBad" + uniqueId, "testDescription" + uniqueId, true, 0);
                TaskDisplayHandler task = MainController.Tasks.Values.FirstOrDefault(x => x.CurrentTask.TaskName == "testCompleteBad" + uniqueId && x.CurrentTask.TaskType == true);

                task.CompleteTask();
                yield return WaitForCondition(() => task.CurrentTask.Completed);
                yield return WaitForCondition(() => baseCurrentScore > User.UserData.Instance.CurrentTaskScore);

                Assert.Greater((TimeSpan.FromMinutes(MainController.Tasks.Last().Value.CurrentTask.PeriodRate) - (DateTime.UtcNow - MainController.Tasks.Last().Value.CurrentTask.LastCompleted)).TotalSeconds, 600);
                Assert.Greater(baseCurrentScore, User.UserData.Instance.CurrentTaskScore);
                Assert.Greater(baseTotalScore, User.UserData.Instance.TotalScore);
            }
        }
    }
}