
#define NUM_COLORS 256

/* Mouse */
#define MOUSE_INT 0x33

#define INIT_MOUSE 0x00
#define SHOW_MOUSE 0x01
#define HIDE_MOUSE 0x02
#define GET_MOUSE_STATUS 0x03

/* Video */
#define VIDEO_INT 0x10

#define SET_MODE 0x00
#define SET_CURSOR 0x02
#define PRINT_CHAR 0x09
#define PRINT_STRING 0x13

#define TEXT_MODE 0x03
#define VGA_256_COLOR_MODE 0x13

#define SCREEN_HEIGHT 200
#define SCREEN_WIDTH 320

#define VRETRACE_BIT 0x08

#define INPUT_STATUS 0x3DA



int main()
{

  return 0;
}