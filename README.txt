Korisni linkovi:
	https://www.c-sharpcorner.com/blogs/alert-message-from-controller-view-using-javascript-alert-messagebox
	https://www.youtube.com/watch?v=QjRLS_Mii9w&list=WL&index=1&t=441s&ab_channel=CodAffection
	https://www.youtube.com/watch?v=vRqz_zUiJTw&ab_channel=IAmTimCorey

Korisne stvari:
	Kada u nekom kontroleru imamo akciju koja vraca:
		return View();
		
	On tada otvara stranicu koja se zove isto kao i ta akcija
	Npr: Naziv akcije je Register, prozor koji se otvara je Register.cshtml
	
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	
	Kada u nekom kontroleru imamo akciju koja vraca:
		return View("../Home/ErrorPage");
		
	On tada otvara stranicu koja se zove ErrorPage i nalazi se u okviru Home kontrolera tj unutar Home foldera
	
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	
	Kada u nekom kontroleru imamo akciju koja vraca:
		return RedirectToAction("../Home/Index");
		
	On tada otvara akciju koja se zove Index i nalazi se u Home kontroleru
	
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	
	Kada u nekom prozoru imamo:
		<a href="/Arrangement/GetDetails?id=@arrangement.ID">Details</a>
		
	a => oznacava da se radi o tagu koji je linkovi
	href => je adresa linka
		Arrangement => predstavlja kontroler
		GetDetails => predstavlja akciju
	? => oznacava da posle njega slede parametri
	id => naziv parametra
	@arrangement.ID => njegova vrednost
	
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////