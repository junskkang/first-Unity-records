<?php  
    //POST 데이터에서 radio 버튼 값 가져오기
    //$selectedValue = isset($_POST["Select"]) ? $_POST["Select"] : null;

    //결과를 확인하기 위한 디버깅용 출력
    if(isset($_POST["Select"]))
    {
        $Idx = $_POST["Select"];
        echo "지울 아이템의 고유번호 : $Idx<br>";

        $con = mysqli_connect("localhost", "junskk", "js752619!!", "junskk");
        if(!$con)
        {
            die("Could not Connect".mysqli_connect_error());    //연결 실패 시 스크립트를 닫겠다는 의미
        }

        $check = mysqli_query($con, "SELECT idx FROM ItemList WHERE idx='{$Idx}'");

        $numrows = mysqli_num_rows($check);

        if ($numrows != 0)
        {
            $result = mysqli_query($con, "DELETE FROM ItemList WHERE idx='{$Idx}'");
            echo "<br><br>선택된 아이템이 삭제 되었습니다.";
        }  
        mysqli_close($con);
    }
    else
    {
        echo "선택된 아이템이 없습니다.";
    }

    echo '<form method="post" action="Item_Input.html">';
    echo '<input type="submit" value="첫페이지로 돌아가기">&nbsp;&nbsp;&nbsp;&nbsp;';    
    echo '</form>';
    echo '<form method="post" action="Item_Print.php">';
    echo '<input type="submit" value="이전페이지로 돌아가기">';    
    echo '</form>';
?>