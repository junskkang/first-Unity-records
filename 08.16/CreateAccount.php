<?php
	$u_id = $_POST["Input_id"];
	$u_pw = $_POST["Input_pw"];
	$nick = $_POST["Input_nick"];

	if( empty($u_id) )
		die("u_id is empty.\n");

	if(empty($u_pw))
		die("u_pw is empty.\n");

	if(empty($nick))
		die("nick is empty.\n");

	//echo "$u_id<br/>";
	//echo "$u_pw<br/>";
	//echo "$nick<br/>";

	$con = mysqli_connect("localhost", "junskk", "js752619!!", "junskk");

	if( !$con)
		die( "Could not connect" . mysqli_connect_error() );

	$check = mysqli_query($con, "SELECT user_id FROM Practice WHERE user_id = '{$u_id}' ");
	$numrows = mysqli_num_rows($check);
	if($numrows != 0)
	{	// 즉 0이 아니라는 뜻은 내가 생성하려고 하는 ID 값이 이미 존재 한다는 뜻이다.
		// (누군가 이미 사용하고 있다는 뜻)
		die("ID does exist.");
	}

	$check = mysqli_query($con, "SELECT nick_name FROM Practice WHERE nick_name = '{$nick}' ");
	$numrows = mysqli_num_rows($check);
	if($numrows != 0)
	{	// 즉 0이 아니라는 뜻은 내가 생성하려고 하는 NickName 값이 이미 존재 한다는 뜻이다.
		// (누군가 이미 사용하고 있다는 뜻)
		die("Nickname does exist.");
	}

	$Result = mysqli_query($con, "INSERT INTO Practice (user_id, user_pw, nick_name) VALUES 
				( '{$u_id}', '{$u_pw}', '{$nick}' );" );

	if($Result)
		echo "Create Success.";
	else 
		echo "Create error.";

	mysqli_close($con);

?> 