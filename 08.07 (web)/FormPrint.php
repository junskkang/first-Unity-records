<meta http-equiv="content-type" content="text/html; charset=utf-8" />

<?
    $id = $_POST["input_ID"];
    $name = $_POST["input_Name"]
    $pw = $_POST["input_Pw"];
    $pwCheck = $_POST["input_PwCheck"]
    $gen = $_POST["gender"];
    $phone1 = $_POST["phone_1"];
    $phone2 = $_POST["phone_2"];
    $phone3 = $_POST["phone_3"];
    $address = $_POST["input_Address"];
    $movie = $_POST["check_Movie"];
    $book = $_POST["check_Book"];
    $shop = $_POST["check_Shopping"];
    $health = $_POST["check_Health"];
    $coment = $_POST["input_SaySomething"];

    echo "아이디 : $id<br />";
    echo "이름 : $name<br />";
    echo "비밀번호 : $pw<br />";
    echo "비밀번호 확인 : $pwCheck<br />";
    echo "성별 : $gen<br />";
    echo "전화번호 : $phone1 - $phone2 - $phone3<br />";
    echo "주소 : $address<br />"; 
    echo "영화 : $movie<br />";
    echo "독서 : $book<br />";
    echo "쇼핑 : $shop<br />";
    echo "운동 : $health<br />";
    echo "자기소개 : $coment<br />";
?>