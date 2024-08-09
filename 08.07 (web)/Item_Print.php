<?php
    $ItName = $_POST["ItemName"];
    $ItLevel = $_POST["ItemLevel"];
    $ItAttrate = $_POST["ItemAttRate"];
    $ItPrice = $_POST["ItemPrice"];

    //DB 접속 시도
    $con = mysqli_connect("localhost", "junskk", "js752619!!", "junskk");
    if(!$con)
    {
        die("Could not Connect".mysqli_connect_error());    //연결 실패 시 스크립트를 닫겠다는 의미
    }

    if($ItName == "" || $ItName == "guest")
    {
        echo "guest 접속 시 아이템 리스트만 출력합니다.<br><br>";
    }

    if($ItName != "" && $ItName != "guest")
    {
        //DB테이블에서 내가 입력한 ID값을 찾는 코드
        $check = mysqli_query($con, "SELECT ItemName FROM ItemList WHERE ItemName='{$ItName}'");

        $numrows = mysqli_num_rows($check);
        if($numrows == 0)   //입력한 아이디와 같은 이름이 데이터 베이스에 없으면
        {
            //정보를 추가해 주는 쿼리문
            //mysqli_query(접속시도 변수, "INSERT INTO 테이블이름 (칼럼이름, 칼럼이름, 칼럼이름) 
                                          //VALUES('{$대입할변수명}', '{$대입할변수명}')");
            $Result = mysqli_query($con, "INSERT INTO ItemList (ItemName, ItemLevel, ItemAttRate, ItemPrice) 
                                VALUES('{$ItName}', '{$ItLevel}', '{$ItAttrate}', '{$ItPrice}')");


            if($Result)
            {
                echo "아이템 추가 성공<br>";
            }
            else
                echo("Create error\n"); //아이템 추가 실패
        }
        else
        {
            echo "이미 존재하는 아이템 이름은 리스트만 출력합니다.<br><br>";
        }
    }

    echo '<form method="post" action="Item_Input.html">';
    echo '<input type="submit" value="이전페이지로 돌아가기">';
    echo '</form>';

    echo '<form name="Item_List" method="post">';

    //10개 리스트 가져오기
    $RowList = mysqli_query($con, "SELECT * FROM ItemList ORDER BY idx DESC LIMIT 0,10");
    if(!$RowList)
    {
        echo "쿼리오류 발생 : ".mysqli_error($con);
    }

    $rowsCount = mysqli_num_rows($RowList);

    echo '<table border="1" width="710" cellspacing="0" cellpadding="4">';
    echo '<tr align="center" bgcolor="blue">';
    echo '<td style="color:white"> 선택 </td>';
    echo '<td style="color:white">고유번호</td>';
    echo '<td style="width:170px; color:white">아이템이름</td>';
    echo '<td style="color:white">레벨</td>';
    echo '<td style="color:white">공격상승률</td>';
    echo '<td style="color:white">가격</td>';
    for($i = 1; $i <= $rowsCount; $i++ )
    {
        $row = mysqli_fetch_array($RowList);
        
        if($row != false)
        {
            $unique = $row["idx"];
            echo '<tr align="center">';
            echo '<td><input type="radio" name="Select" value="'.$row["idx"].'"></td>';
            echo '<td>';
            echo $row["idx"];
            echo '</td>';
            echo '<td style="width:170px"">';
            echo $row["ItemName"];
            echo '</td>';
            echo '<td>';
            echo $row["ItemLevel"];
            echo '</td>';
            echo '<td>';
            echo $row["ItemAttRate"];
            echo '</td>';
            echo '<td>';
            echo $row["ItemPrice"];
            echo '</td>';
            echo '</tr>';
        }      
    }
    echo '</table><br><br>';
    

    echo '<form method="post" action="Item_Input.html">';
    echo '<input type="submit" value="이전페이지로 돌아가기">&nbsp;&nbsp;&nbsp;&nbsp;';
    echo '<input type="submit" value="선택 아이템 삭제" formaction="Item_Del.php">';
    echo '</form>';
    echo '</form>';

    //echo $row["idx"]."번아이템 : ".$row["ItemName"]." : 레벨 ".$row["ItemLevel"]." : 공격상승률 "
    //            .$row["ItemAttRate"]." : 가격 ".$row["ItemPrice"]."<br>";

    mysqli_close($con);
?>

