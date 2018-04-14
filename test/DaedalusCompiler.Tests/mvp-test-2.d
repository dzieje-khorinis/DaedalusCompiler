// these 4 symbols are required by original parser
func void startup_global() {};
func void init_global() {};
class C_Npc { var int data [200]; };
instance PC_HERO(C_Npc) {};

// we want support for constants
const int answer = 42;

// we want to define simple functions
// this is first script function called by engine
func void initPerceptions()
{
  // we want to call engine functions
  // we want to reference constants
	playVideo(intToString(answer); // this will be logged to zSpy
	exitSession();
};
