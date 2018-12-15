using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Threading.Tasks;

namespace Server
{
    public class phatSinh16
    {
        Random rnd = new Random();
        //Ma trận phần tử đầy đủ
        List<List<int>> matrix_full = new List<List<int>>();
        //Ma trận phần tử đục lỗ
        List<List<int>> matrix_half = new List<List<int>>();

        #region STEP1:LAM_DAY_LUOI_SO         //Step1: Làm đầy lưới số

        //Làm đầy Hàng thứ nhất của matrix_full
        void Completed_R0()
        {

            //Resource: 1-16
            List<int> resource = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };
            matrix_full = new List<List<int>>();
            matrix_half = new List<List<int>>();
            for (int i = 0; i < 16; i++)
            {
                int number = rnd.Next(0, resource.Count);


                matrix_full.Add(new List<int>());
                matrix_full[0].Add(resource[number]);
                matrix_half.Add(new List<int>());
                matrix_half[0].Add(resource[number]);


                resource.RemoveAt(number);
            }

            //recovery resource
            for (int i = 0; i < 16; i++)
            {
                resource.Add(i + 1);
            }
        }

        //Làm đầy các hàng còn lại của matrix_full
        void Completed_All_R()
        {

            for (int i = 1; i < 16; i++)
            {
                //Không chia hết cho 4, dùng xoay vòng 4slot
                if (i % 4 != 0)
                {


                    circular_shift4(matrix_full[i - 1], matrix_full[i]);
                    circular_shift4(matrix_half[i - 1], matrix_half[i]);


                }
                //Chia hết cho 4, dùng xoay vòng 3 slot
                else
                {


                    circular_shift3(matrix_full[i - 1], matrix_full[i]);
                    circular_shift3(matrix_half[i - 1], matrix_half[i]);


                }
            }

        }

        #region Xoay_vong       //Xoay vong 3 slot & 4 slot

        //Xoay vòng 4 slot
        void circular_shift4(List<int> lscurrent, List<int> lsafter)
        {
            for (int i = 0; i < 16; i++)
            {
                lsafter.Add(lscurrent[(i + 4) % 16]);
            }
        }

        //Xoay vòng 3 slot
        void circular_shift3(List<int> lscurrent, List<int> lsafter)
        {
            for (int i = 0; i < 16; i++)
            {
                lsafter.Add(lscurrent[(i + 3) % 16]);
            }
        }

        #endregion
        #endregion
        #region STEP2:DUC_SO_SO         //STEP2: Đục lỗ số
        //Xóa ngẫu nhiên
        void delete_random()
        {
            //List này chứ index (từ 0-81) của ô được đục lỗ
            List<int> list = new List<int>();
            for (int i = 0; i < 256; i++)
            {
                list.Add(i);
            }

            //Chọn ô đục lỗ
            while (list.Count > 0)
            {
                //randomValue: index của vị trí được chọn
                //value: giá trị của vị trí được chọn
                int randomValue = rnd.Next(0, list.Count);
                int value = list[randomValue];


                //tọa độ số được chọn
                //row: hàng
                //col: cột
                int row = value / 16;
                int col = value % 16;
                //Xóa index của số đã được chọn (không random vào giá trị này nữa)
                list.RemoveAt(randomValue);


                //xác định vị trí của số đối xứng
                int row2 = find_Box(row, col)[0];
                int col2 = find_Box(row, col)[1];
                //Xóa index của số đối xứng (không random vào giá trị này nữa)
                int index = row2 * 16 + col2;
                list.Remove(index);



                //gán 2 giá trị được chọn bằng 0
                matrix_half[row][col] = 0;
                matrix_half[row2][col2] = 0;
                //backup lại 2 giá trị cũ
                int number_backup1 = matrix_full[row][col];
                int number_backup2 = matrix_full[row2][col2];



                //xét điều kiện để xóa phần tử
                //CheckForRemove = true: xóa phần tử, count = count - 1
                //CheckForRemove = false: hoàn trả lại phần tử ban đầu
                if (!CheckForRemove(row, col) && !CheckForRemove(row2, col2))
                {
                    matrix_half[row][col] = number_backup1;
                    matrix_half[row2][col2] = number_backup2;
                }
            }

        }


        //Tìm phần tử đối xứng
        int[] find_Box(int row, int col)
        {
            int row_result;
            int col_result;


            //find row
            row_result = 15 - row;

            //find col
            col_result = 15 - col;


            int[] arrResult = new int[2];
            arrResult[0] = row_result;
            arrResult[1] = col_result;
            return arrResult;
        }

        //Điều kiện xóa ô
        Boolean CheckForRemove(int row, int col)
        {
            int count = 1;
            for (int i = 1; i < 15; i++)
            {
                if (Cols_Constrants(row, col, i) && Rows_Constrants(row, col, i) && Box_Constrants(row, col, i))
                    count--;
            }

            if (count < 0)
                return false;
            else
                return true;
        }


        //Ràng buộc dòng
        Boolean Rows_Constrants(int row, int col, int num)
        {
            for (int i = 0; i < 16; i++)
            {
                if (i != col && matrix_half[i][col] != 0 && num == matrix_half[i][col])
                {
                    return false;
                }
            }
            return true;
        }
        //Ràng buộc cột
        Boolean Cols_Constrants(int row, int col, int num)
        {
            for (int i = 0; i < 16; i++)
            {
                if (i != row && matrix_half[row][i] != 0 && num == matrix_half[row][i])
                {
                    return false;
                }
            }
            return true;
        }
        //Ràng buộc ô lớn
        Boolean Box_Constrants(int row, int col, int num)
        {

            int rMin = xd_Row(row)[0];
            int rMax = xd_Row(row)[1];
            int cMin = xd_Col(col)[0];
            int cMax = xd_Col(col)[1];


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


        //Module của ràng buộc ô lớn
        //Xác định dòng
        int[] xd_Row(int row)
        {
            int temp = row / 4;
            int Rmin = temp * 4;
            int Rmax = Rmin + 3;

            int[] arrResult = new int[2];
            arrResult[0] = Rmin;
            arrResult[1] = Rmax;
            return arrResult;
        }
        //Xác định cột
        int[] xd_Col(int col)
        {
            int temp = col / 4;
            int Cmin = temp * 4;
            int Cmax = Cmin + 3;

            int[] arrResult = new int[2];
            arrResult[0] = Cmin;
            arrResult[1] = Cmax;
            return arrResult;
        }

        #endregion
        //Ham phat sinh
        public int[,] phatsinh()
        {
            int[,] arr = new int[16, 16];
            Completed_R0();
            Completed_All_R();
            delete_random();
            for (int i = 0; i < 16; i++)
            {
                for (int j = 0; j < 16; j++)
                {
                    arr[i, j] = matrix_half[i][j];
                }
            }
            return arr;
        }

    }
}