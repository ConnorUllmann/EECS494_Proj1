#include <iostream>
#include <fstream>
#include <string>
#include <vector>

using namespace std;

//Converts one line of tile data from Ogmo Editor output to Unity input.
string Modify(string s)
{
	string o;
	
	int l = -1, d = -1;
	while(1)
	{
		l = d+1;
		d = s.find(',', l);
		if(d == string::npos)
		{
			return o;
		}
		else
		{
			string o_n = s.substr(l, d-l);
			while(o_n.length() < 3)
				o_n.insert(0, "0");
			o += (l == 0 ? "" : " ") + o_n;
		}
	}

	return o;
}

void PrintTiles(string inputFilename)
{
	ifstream in(inputFilename.c_str());

	string filename;
	cout << "Enter output filename: ";
	cin >> filename;
    ofstream out(filename.c_str());

	vector<string> all;
	bool modify = false;
    string line;
    do
    {
		//Find the "Tiles" xml tag and begin converting lines of tile data
        getline(in, line);
		if(line.find("<Tiles") != string::npos)
		{
			int ind = line.find(">")+1;
			line = line.substr(ind, line.length() - ind);
			modify = true;
		}
		//Find the "Tiles" xml end tag to stop converting lines of tile data
		else if(line.find("</Tiles>") != string::npos)
		{
			int ind = line.find("<");
			line = line.substr(0, ind);
			modify = false;
		}
		
		if(modify)
			all.push_back(Modify(line));
		
    } while(in);

	//Output all of the lines one at a time in reverse order
	//(Ogmo Editor outputs top-to-bottom while Unity inputs bottom-to-top
	for(int i = all.size()-1; i>= 0; i--)
	{
		out << all[i];
		if(i > 0)
			out << endl;
	}

	in.close();
	out.close();
}

int main(int argc, char *argv[])
{
    PrintTiles(argv[1]);
    return 0;
}
