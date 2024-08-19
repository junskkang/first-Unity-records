<head>
	<meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
</head>


<?
	$u_id = $_POST["Input_id"];
	$u_pw = $_POST["Input_pw"];

	//echo "$u_id<br>";
	//echo "$u_pw<br>";

	$con = mysqli_connect("Localhost", "junskk", "js752619!!", "junskk");
	//"Localhost" 같은 서버 내에 있는 DB를 찾겠다는 의미

	if(!$con)
	die("Could not connect".mysqli_connect_error());

	//입력한 아이디가 데이터베이스에 저장된 아이디와 동일한지 확인
	$check = mysqli_query($con, "SELECT * FROM Practice WHERE user_id='{$u_id}'");

	$numrows = mysqli_num_rows($check);
	if($numrows == 0){
		die("ID does not exist");
	}

	

	$row = mysqli_fetch_assoc($check);	//user_id 이름에 해당하는 행의 내용을 가져온다.
	if($row){
		if($u_pw == $row["user_pw"])	//비밀번호도 맞다면
		{
			//echo "아이디 : ".$row["user_id"]."<br>";
			//echo "별명 : ".$row["nick_name"]."<br>";
			//echo "최고기록 : ".$row["best_score"]."<br>";
			//echo "게임머니 : ".$row["game_gold"]."<br>";
			//echo "현재 층 : ".$row["floor_info"]."<br>";

			// PHP에서 JSON 생성 코드
			$ArrData = array();
			$ArrData["nick_name"]  = $row["nick_name"];
			$ArrData["best_score"]  = $row["best_score"];
			$ArrData["game_gold"] = $row["game_gold"];
			$ArrData["floor_info"]   = $row["floor_info"];
			$ArrData["info"]  	      = $row["info"];
			$output = json_encode($ArrData, JSON_UNESCAPED_UNICODE);


			// JSON으로 변환
			header('Content-Type: application/json');
			echo $output;
			echo ("\n");
			echo "Login_Success!!";
		}
		else{
			die("Password does not Match");	//비밀번호 오류
		}
	}

	//echo "DB 접속 성공 및 유저정보 찾기 성공";

	mysqli_close($con);
?>