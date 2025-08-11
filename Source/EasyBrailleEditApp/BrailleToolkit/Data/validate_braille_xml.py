import xml.etree.ElementTree as ET
import sys
import os

def hex_to_dots(hex_code):
    """
    Converts a single 2-character hex code to a dots string.
    e.g., "2A" -> "246"
    """
    if not hex_code or len(hex_code) != 2:
        return ""

    try:
        byte_val = int(hex_code, 16)
    except ValueError:
        return f"[Invalid Hex: {hex_code}]"

    if byte_val == 0:
        return "" # Represents a blank cell, but for comparison we'll see.

    dots = []
    if (byte_val & 1) == 1: dots.append('1')
    if (byte_val & 2) == 2: dots.append('2')
    if (byte_val & 4) == 4: dots.append('3')
    if (byte_val & 8) == 8: dots.append('4')
    if (byte_val & 16) == 16: dots.append('5')
    if (byte_val & 32) == 32: dots.append('6')
    
    return "".join(dots)

def convert_code_to_dots_string(code_attr):
    """
    Converts a full 'code' attribute string (which can contain multiple cells)
    to a full 'dots' attribute string.
    e.g., "2A15" -> "246 135"
    """
    if code_attr is None:
        return None
    
    if code_attr == "00":
        return " " # Special case for blank cell

    # Split the code into 2-character chunks
    codes = [code_attr[i:i+2] for i in range(0, len(code_attr), 2)]
    
    # Convert each chunk to dots
    dot_parts = [hex_to_dots(c) for c in codes]
    
    return " ".join(dot_parts)

def validate_file(file_path):
    """
    Validates a single XML file.
    """
    if not os.path.exists(file_path):
        print(f"Error: File not found at '{file_path}'")
        return False

    print(f"--- Validating {os.path.basename(file_path)} ---")
    
    try:
        tree = ET.parse(file_path)
        root = tree.getroot()
    except ET.ParseError as e:
        print(f"Error: Could not parse XML file. Details: {e}")
        return False

    all_valid = True

    for elem in root.findall('.//symbol'):
        # --- Validate code vs dots ---
        code_val = elem.get('code')
        dots_val = elem.get('dots')

        if code_val is not None:
            if dots_val is None:
                print(f"Validation FAILED: Element has 'code' but no 'dots' attribute.")
                print(ET.tostring(elem, encoding='unicode'))
                all_valid = False
                continue

            expected_dots = convert_code_to_dots_string(code_val)
            if expected_dots != dots_val:
                print(f"Validation FAILED: Mismatch for 'code' attribute.")
                print(f"  - Element: {ET.tostring(elem, encoding='unicode').strip()}")
                print(f"  - code='{code_val}'")
                print(f"  - Expected dots='{expected_dots}'")
                print(f"  - Found    dots='{dots_val}'")
                print("-" * 20)
                all_valid = False

        # --- Validate code2 vs dots2 ---
        code2_val = elem.get('code2')
        dots2_val = elem.get('dots2')

        if code2_val is not None:
            if dots2_val is None:
                print(f"Validation FAILED: Element has 'code2' but no 'dots2' attribute.")
                print(ET.tostring(elem, encoding='unicode'))
                all_valid = False
                continue
            
            expected_dots2 = convert_code_to_dots_string(code2_val)
            if expected_dots2 != dots2_val:
                print(f"Validation FAILED: Mismatch for 'code2' attribute.")
                print(f"  - Element: {ET.tostring(elem, encoding='unicode').strip()}")
                print(f"  - code2='{code2_val}'")
                print(f"  - Expected dots2='{expected_dots2}'")
                print(f"  - Found    dots2='{dots2_val}'")
                print("-" * 20)
                all_valid = False

    if all_valid:
        print("Validation PASSED.")
    else:
        print("Validation FINISHED with errors.")
        
    print("\n")
    return all_valid

if __name__ == "__main__":
    script_dir = os.path.dirname(os.path.abspath(__file__))
    
    print(f"Scanning for XML files in: {script_dir}\n")
    
    xml_files = [f for f in os.listdir(script_dir) if f.endswith('.xml')]
    
    if not xml_files:
        print("No XML files found in this directory.")
        sys.exit(0)
        
    total_failed_files = 0
    for filename in xml_files:
        file_path = os.path.join(script_dir, filename)
        if not validate_file(file_path):
            total_failed_files += 1
            
    print("=" * 40)
    print("Overall Validation Summary")
    print("=" * 40)
    if total_failed_files == 0:
        print(f"SUCCESS: All {len(xml_files)} XML files passed validation.")
        sys.exit(0)
    else:
        print(f"FAILURE: {total_failed_files} out of {len(xml_files)} file(s) failed validation.")
        sys.exit(1)