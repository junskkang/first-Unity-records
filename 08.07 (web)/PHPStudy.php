<html>
	<head>
		<title>PHPStudy_2024_08_08</title>
	</head>
	<body>
		<h2>PHP 문법 연습</h2>
		<?php
			//1. 출력하기
			echo "반가워요 php<br>";
			print "print 함수로 출력<br>";

			//2. 변수 선언하기
			//PHP에서는 변수의 데이터형이 별도로 존재하지 않는다.
			//var형과도 다른게 var는 사용해놓고 한번 지정된 데이터형으로 고정이 되지만
			//PHP에서는 변수의 데이터형을 계속 바꿔가며 사용할 수 있다.

			//PHP에서 변수를 사용하기 위해서는 변수명 앞에 $표기를 붙여줘야 한다.
			echo "<br><br>";

			$name = "드래곤";
			$data1 = 1+ 2;
			$data2 = $data1 / 4;

			echo $name."<br>";	//.은 문자열과 다른 것을 합쳐달라는 의미.. +
			echo $data1."<br>";	//data1과 문자열이 합쳐서 출력되게 됨
			echo "$data2<br>";

			$name = 123 + 2;
			echo "$name<br><br>";	//변수의 재활용! 데이터형의 제한을 받지 않음


			//3. 문자열 연결 연산자 도트 (.)
			echo "<br><br>";

			$data1 = "천사의 ";
			$data2 = $data1."반지";

			echo $data2;

			$name = "다니엘";
			echo "<br>나의 이름은 ".$name."입니다";
			echo "<br><br>";

			//쌍따옴표와 작은따옴표 사용하기 (둘 다 문자열로 인식함)
			$AAA = "아기상어";
			$BBB = '어른상어';
			echo $AAA." : ".$BBB;

			$a = 111;	//정수값 저장
			echo "<br>$a";	//""는 출력시에 변수 안의 내용이 출력이 됨
			$a = 321;
			echo '<br>$a';	//''는 출력시 변수 이름 자체가 문자열로 출력이 됨


			//4. 변수의 형변환
			echo "<br><br>";
			$data1 = 3.14;
			echo $data1."<br>";
			$data2 = (int)$data1;	//소숫점 아래는 사라지고 정수만 남게 됨
			echo $data2;	

			//형변환 가능 타입들
			//(int)(integer) : 정수형
			//(float)(double)(real) : 실수형
			//(string) : 문자열로 변환
			//(array) : 배열로 변환
			//(object) : 객체로 변환

			//5. 연산자
			echo "<br><br>";
			$data1 = 5 + 7;
			$data2 = 32 / (1+2);
			$data3 = $data4 = 3;

			echo $data1."<br>";
			echo $data2."<br>";
			echo $data3."<br>";
			echo $data4."<br>";
			echo $data1 - $data2."<br>";

			//6. for문 예제
			echo "<br><br>";
			$sum = 0;
			for($i = 1; $i <= 10; $i++){
				$sss = $i;
				$sum += $i;
				echo "for문 결과 누적 : $sum<br>";
			}
			echo "sss변수의 값 $sss<br>";
			//for문 안에서 만든 $sss 지역변수인데 해당 지역을 벗어나도 사용이 가능함

			//7. break문을 사용해서 while반복문을 탈출하는 예제
			echo "<br><br>";
			$k = 0;
			while(1){
				$k++;
				if(10 < $k)
					break;

				$x = $k;
			}
			echo "변수 x에 저장한 값은 $x 입니다";

			//8.배열
			echo "<br><br>";
			$AAA = array();		//배열 선언
			$AAA[0] = 34;		//인덱스 지정 후 값 대입
			$AAA[1] = 58;
			$AAA[2] = 123;

			$Arr[] = 42;		//자동으로 인덱스 0번에 값 대입
			$Arr[] = 73;		//자동으로 인덱스 1번에 값 대입
			$Arr[] = 100;		//자동으로 인덱스 2번에 값 대입
			$Arr[0] = 37;		//인덱스 0번에 37 덮어씌우기
			$Arr[2] = 25;		//인덱스 2번에 25 덮어씌우기

			echo $Arr[0]."<p>";
			echo $Arr[1]."<p>";
			echo $Arr[2]."<p>";

			$fruit["a"] = "사과";		//인덱스에 문자열도 사용 가능
			$fruit["b"] = "바나나";
			
			echo $fruit["a"]."<p>";
			echo $fruit["b"]."<p>";

			echo "배열의 갯수 : ".count($Arr);	//배열의 갯수를 구하는 함수

			//9.배열의 암시적 선언
			echo "<br><br>";
			$hobby = array("영화감상", "등산", "게임");
			//3개의 값을 배열에 등록하여 hobby 배열 변수 만들기
			echo $hobby[0]."<br>";
			echo $hobby[1]."<br>";
			echo $hobby[2]."<br>";

			//10. 배열의 참조 변수 만들기 &
			echo "<br><br>";
			$brray = array();
			$score = &$brray;		//$brray라는 배열 변수의 참조 형태 변수가 됨
			$score[0] = 24;
			$score[1] = 35;
			$score[2] = 10;

			echo $brray[0]."<br>";
			echo $brray[1]."<br>";
			echo $brray[2]."<br>";

			//11. 배열 암시적 선언, 지정 인덱스
			echo "<br><br>";
			$flower = array("장미", "개나리", "진달래", 2 => "해바라기", "튤립");
			//2번 인덱스를 지정하여 그곳에다가 해바라기를 넣어줌
			echo $flower[0]."<p>";	//장미
			echo $flower[1]."<p>";	//개나리	
			echo $flower[2]."<p>";	//해바라기
			echo $flower[3]."<p>";	//튤립

			//12. 함수 : 두변수를 합산해서 출력하는 예제
			function plus($a, $b)	//a,b의 변수값을 받아서 plus하는 함수의 정의
			{
				$c = $a + $b;	//전달받은 a,b를 합산하여 변수c에 대입ㄷ
				echo $c."<p>";
			}
			plus(5, 19);
			plus(4, 34);

			echo "c변수 출력 : ".$C. "<br>"; //함수 내의 지역변수는 접근할 수 없다.

			function Multiple($a, $b){
				$c = $a * $b;
				return $c;
			}

			$result = Multiple(11,45);
			echo $result." : 멀티플 함수의 결과<br>";

			function DivRest($a, $b)	//목과 나머지를 구하는 함수
			{
				$div = intval($a / $b);		// a/b의 값을 정수형으로 반환 : 몫
				$rest = $a % $b;			// a/b의 값을 나눈 후 나머지 값을 변수에 할당

				return array($div, $rest);
			}

			$MyArr = DivRest(30,7);
			echo "몫 : ".$MyArr[0].", 나머지 : ".$MyArr[1]."<br>";

			//13. 지역변수를 전역변수로 선언하는 global 키워드 함수 예제
			echo "<br><br>";
			$data1 = "전역변수";

			//변수명은 같지만 결과적으로 함수 내에서 사용된 $data1과 
			//바깥의 $data1은 다른 변수라고 볼 수 있다.
			function MyFunc2()
			{
				$data1 = "정말 글로벌 변수가 맞나?";
				echo "$data1<br>";	
			}
			MyFunc2();	//정말 글로벌 변수가 맞나?	

			echo $data1;	//전역변수

			echo "<br><br>";
			function MyFunc3()
			{
				global $data1;	//여기에서 $data1 변수는 글로벌 변수를 의미한다.
				$data1 = "정말 글로벌 변수가 맞나?";
				echo "$data1<br>";
			}
			MyFunc3();		//정말 글로벌 변수가 맞나?	

			echo $data1;    //정말 글로벌 변수가 맞나?	

			//14. PHP에서 MySQL 사용을 위한 함수들
			//die() : PHP스크립트의 실행을 즉시 중지시켜주는 함수
			//mysqli_connect() : MySQL 데이터 베이스에 연결시켜주는 함수
			//mysqli_connect_error() : MySQL 서버의 접근 오류를 반환하는 함수
			//mysqli_query() : SQL 명령어 (쿼리문)을 실행하는 함수
			//mysqli_num_rows() : 데이터베이스에서 쿼리를 보내서 나온 레코드의 개수를 알아낼 때 쓰임
			//해당 값이 0이라면, 내가 찾는 행의 값이 없다는 의미
			//mysqli_fetch_assoc() : MySQL의 실행 결과에서 결과 행을 가져오는 함수
			//mysqli_close() : MySQL 서버의 연결을 종료하는 함수
		?>
	</body>
</html>