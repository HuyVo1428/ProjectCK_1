using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    public class phatSinh9
    {
        List<List<int>> matrix_full = new List<List<int>>();
        List<List<int>> matrix_half = new List<List<int>>();
        #region Lam_Day_Matrix
        //Làm đầy hàng 1
        void Completed_R0()
        {
            List<int> resource = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            matrix_full = new List<List<int>>();
            matrix_half = new List<List<int>>();
            Random rnd;
            for (int i = 0; i < 9; i++)
            {
                rnd = new Random();
                int number = rnd.Next(0, resource.Count);

                matrix_full.Add(new List<int>());
                matrix_full[0].Add(resource[number]);

                matrix_half.Add(new List<int>());
                matrix_half[0].Add(resource[number]);

                resource.RemoveAt(number);
                //MessageBox.Show(Convert.ToString(matrix_full[0][i]));
            }

            //recovery resource
            //for (int i = 0; i < 9; i++)
            //{
            //    resource.Add(i + 1);
            //}
        }
        //Làm đầy các hàng còn lại của matrix_full
        void Completed_All_R()
        {
            for (int i = 1; i < 9; i++)
            {
                //Không chia hết cho 3, dùng xoay vòng 3slot
                if (i % 3 != 0)
                {
                    circular_shift3(matrix_full[i - 1], matrix_full[i]);
                    circular_shift3(matrix_half[i - 1], matrix_half[i]);

                }
                //Chia hết cho 3, dùng xoay vòng 2 slot
                else
                {
                    circular_shift1(matrix_full[i - 1], matrix_full[i]);
                    circular_shift1(matrix_half[i - 1], matrix_half[i]);
                }
            }

        }
        //Xoay vòng 3 slot
        void circular_shift3(List<int> lscurrent, List<int> lsafter)
        {
            for (int i = 0; i < 9; i++)
            {
                lsafter.Add(lscurrent[(i + 3) % 9]);
            }
        }
        //Xoay vòng 2 slot
        void circular_shift1(List<int> lscurrent, List<int> lsafter)
        {
            for (int i = 0; i < 9; i++)
            {
                lsafter.Add(lscurrent[(i + 2) % 9]);
            }
        }
        #endregion
        //Check điều kiện
        #region Dieu_Kien_Rang_Buoc và xóa ô
        Boolean Rows_Constrants(int row, int col, int num)
        {
            for (int i = 0; i < 9; i++)
            {
                if (i != col && matrix_half[i][col] != 0 && num == matrix_half[i][col])
                {
                    return false;
                }
            }
            return true;
        }
        Boolean Cols_Constrants(int row, int col, int num)
        {
            for (int i = 0; i < 9; i++)
            {
                if (i != row && matrix_half[row][i] != 0 && num == matrix_half[row][i])
                {
                    return false;
                }
            }
            return true;
        }
        Boolean Box_Constrants(int row, int col, int num)
        {

            int rMin = int.Parse(xd_Row(row)[0].ToString());
            int rMax = int.Parse(xd_Row(row)[1].ToString());
            int cMin = int.Parse(xd_Col(col)[0].ToString());
            int cMax = int.Parse(xd_Col(col)[1].ToString());


            for (int i = rMin; i < rMax + 1; i++)
            {
                for (int j = cMin; j < cMax + 1; j++)
                {
                    if (i != row && j != col && matrix_half[i][j] != 0 && num == matrix_half[i][j])
                    {
                        return false;
                    }
                }
            }


            return true;
        }
        //Module của Boxconstrants
        #region Module_cua_Boxconstrants
        string xd_Row(int row)
        {
            int temp = row / 3;
            int Rmin = temp * 3;
            int Rmax = Rmin + 2;

            return Rmin.ToString() + Rmax.ToString();
        }

        string xd_Col(int col)
        {
            int temp = col / 3;
            int Cmin = temp * 3;
            int Cmax = Cmin + 2;

            return Cmin.ToString() + Cmax.ToString();
        }
        #endregion

        // Check điều kiện xóa ô
        Boolean CheckForRemove(int row, int col)
        {
            int count = 1;
            for (int i = 1; i < 9; i++)
            {
                if (Cols_Constrants(row, col, i) && Rows_Constrants(row, col, i) && Box_Constrants(row, col, i))
                    count--;
            }

            if (count < 0)
                return false;
            else
                return true;
        }
        //Tìm phần tử đối xứng
        string find_Box(int row, int col)
        {
            int row_result;
            int col_result;


            //find row
            row_result = 8 - row;

            //find col
            col_result = 8 - col;


            return row_result.ToString() + col_result.ToString();
        }
        //Xóa ngẫu nhiên
        void delete_ranfdom()
        {
            Random rnd;
            //List này chứ index (từ 0-81) của ô được đục lỗ
            List<int> list = new List<int>();
            for (int i = 0; i < 81; i++)
            {
                list.Add(i);
            }

            //Chọn ô đục lỗ
            while (list.Count > 0)
            {
                rnd = new Random(DateTime.Now.Second);
                int randomValue = rnd.Next(0, list.Count);
                int value = list[randomValue];


                //Số được chọn
                int row = value / 9;
                int col = value % 9;
                //Xóa index của số đã được chọn (không random vào giá trị này nữa)
                list.RemoveAt(randomValue);


                //Số đối xứng
                int row2 = int.Parse(find_Box(row, col)[0].ToString());
                int col2 = int.Parse(find_Box(row, col)[1].ToString());
                //Xóa index của số đối xứng (không random vào giá trị này nữa)
                int index = row2 * 9 + col2;
                list.Remove(index);




                matrix_half[row][col] = 0;
                matrix_half[row2][col2] = 0;
                int number_backup1 = matrix_full[row][col];
                int number_backup2 = matrix_full[row2][col2];



                //CheckForRemove = true: xóa phần tử, count = count - 1
                //CheckForRemove = false: hoàn trả lại phần tử ban đầu
                if (!CheckForRemove(row, col) || !CheckForRemove(row2, col2))
                {
                    matrix_half[row][col] = number_backup1;
                    matrix_half[row2][col2] = number_backup2;
                }
            }

        }
        #endregion


        //Ham phat sinh
        public int[,] phatsinh()
        {
            int[,] arr = new int[9, 9];
            Completed_R0();
            Completed_All_R();
            delete_ranfdom();
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    arr[i, j] = matrix_half[i][j];
                }
            }
            return arr;
        }
    }
}
