using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AT3
{
    [Serializable()]
    public class Queue
    {
        private List<QueueNode> nodeList;
        private QueueNode head;

        public Queue()
        {
            nodeList = new List<QueueNode>();
            head = null;
        }
        public Queue(List<QueueNode> _nodeList)
        {
            if (_nodeList.Count > 0)
            {
                nodeList = _nodeList;
                head = nodeList[0];

                for (int i = 0; i < nodeList.Count - 1; i++)
                {
                    if (nodeList[i] != null)
                    {
                        nodeList[i].NextNode = nodeList[i + 1];
                    }
                    else
                    {
                        i = int.MaxValue;
                    }
                }

                nodeList[nodeList.Count - 1].NextNode = null;
            }
            else
            {
                new Queue();
            }
        }
        public List<QueueNode> NodeList
        {
            get => nodeList;
            //set => nodeList = value;
        }
        public QueueNode Head
        {
            get => head;
            set => head = value;
        }
        public bool completeTask()
        {
            // cannot set head to head.next if head is null
            bool isSuccess = (head != null);

            if (isSuccess)
            {
                QueueNode temp = head;
                nodeList.RemoveAt(0);
                //temp = head.NextNode;
                head = temp.NextNode;
            }

            return isSuccess;
        }
    }
    [Serializable()]
    public class QueueNode
    {
        private Task data;
        private QueueNode nextNode;

        public QueueNode(Task _data)
        {
            data = _data;
            nextNode = null;
        }

        public Task Data
        {
            get => data;
            //set => data = value;
        }
        public QueueNode NextNode
        {
            get => nextNode;
            set => nextNode = value;
        }
    }
    public class QuickSort
    {
        public static List<QueueNode> quickSort(QueueNode[] array)
        {
            int low = 0;
            int high = array.Length - 1;

            // ie. low != high
            if (low < high)
            {
                int pivot = qsPartition(array, low, high);
                qsRecursive(array, low, pivot - 1);
                qsRecursive(array, pivot + 1, high);
            }

            return ((QueueNode[])array).ToList();
        }

        private static void qsRecursive(QueueNode[] array, int low, int high)
        {
            // ie. low != high
            if (low < high)
            {
                int pivot = qsPartition(array, low, high);
                qsRecursive(array, low, pivot - 1);
                qsRecursive(array, pivot + 1, high);
            }
        }

        // high becomes pivot
        private static int qsPartition(QueueNode[] array, int low, int pivot)
        {
            while (low < pivot)
            {
                // number greater than pivot
                if (array[low].Data.ID > array[pivot].Data.ID)
                {
                    qsSwap(array, low, pivot);
                    pivot--;
                }
                // number less than pivot
                else
                {
                    low++;
                }
            }

            // new pivot index
            return pivot;
        }

        private static void qsSwap(QueueNode[] array, int low, int high)
        {
            QueueNode temp = array[low];
            array[low] = array[high - 1];
            array[high - 1] = array[high];
            array[high] = temp;
        }
    }
}
