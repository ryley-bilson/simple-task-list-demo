using System.Collections.Generic;

namespace SimpleTaskListDemo
{
    public class MyQuicksort<T> where T : System.IComparable
    {
        // TODO: make generic
        public static List<T> quickSort<T>(List<T> array) where T : System.IComparable
        {
            int low = 0;
            int high = array.Count - 1;

            // ie. low != high
            if (low < high)
            {
                int pivot = qsPartition(array, low, high);
                qsRecursive(array, low, pivot - 1);
                qsRecursive(array, pivot + 1, high);
            }

            return array;
        }

        private static void qsRecursive<T>(List<T> array, int low, int high) where T : System.IComparable
        {
            // ie. low != high
            if (low < high)
            {
                // high becomes pivot
                int pivot = qsPartition(array, low, high);
                qsRecursive(array, low, pivot - 1);
                qsRecursive(array, pivot + 1, high);
            }
        }
        /// <summary>
        /// Within the given partition (ie. array[low] to array[pivot]), rearrange such that all values less than pivot are positioned at lower indices, and all values greater than pivot are positioned are higher indices.
        /// </summary>
        /// <param name="array"></param>
        /// <param name="low">Lower index bound of partition in array.</param>
        /// <param name="pivot">Upper index bound of partition in array, used as pivot.</param>
        /// <returns></returns>
        private static int qsPartition<T>(List<T> array, int low, int pivot) where T : System.IComparable
        {
            while (low < pivot)
            {
                // number greater than pivot
                if (array[low].CompareTo(array[pivot]) > 0)
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
        /// <summary>
        /// Pivot/high is moved down an index, element below high swaps with low.
        /// </summary>
        /// <param name="array"></param>
        /// <param name="low">Index with element of value greater than high's element value.</param>
        /// <param name="high">Pivot index.</param>
        private static void qsSwap<T>(List<T> array, int low, int high)
        {
            T temp = array[low];

            // NOTE: assuming each (actual) line takes same effort/processing power/time, 
            //       [if/else] becomes [4/3] lines instead of [3/3]
            //// Element below pivot/high is NOT low
            //if (low != high - 1)
            //{
            //    array[low] = array[high - 1];
            //    array[high - 1] = array[high];
            //}
            //else
            //{
            //    array[low] = array[high];
            //}
            //array[high] = temp;

            array[low] = array[high - 1];
            array[high - 1] = array[high];
            array[high] = temp;
        }
    }
}
