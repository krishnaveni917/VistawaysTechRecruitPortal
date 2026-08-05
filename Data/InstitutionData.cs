namespace VistaWaysTechRecruitPortal.Data
{
    public static class InstitutionData
    {
        public static Dictionary<string, string> Universities = new()
        {
            // Indian Universities
            ["Jawaharlal Nehru Technological University Hyderabad"] = "Hyderabad, Telangana",
            ["Jawaharlal Nehru Technological University Anantapur"] = "Anantapur, Andhra Pradesh",
            ["Osmania University"] = "Hyderabad, Telangana",
            ["Andhra University"] = "Visakhapatnam, Andhra Pradesh",
            ["Sri Venkateswara University"] = "Tirupati, Andhra Pradesh",
            ["Kakatiya University"] = "Warangal, Telangana",
            ["University of Hyderabad"] = "Hyderabad, Telangana",
            ["University of Delhi"] = "New Delhi, Delhi",
            ["Jawaharlal Nehru University"] = "New Delhi, Delhi",
            ["Indian Institute of Technology Delhi"] = "New Delhi, Delhi",
            ["Indian Institute of Technology Bombay"] = "Mumbai, Maharashtra",
            ["Indian Institute of Technology Madras"] = "Chennai, Tamil Nadu",
            ["Indian Institute of Technology Kharagpur"] = "Kharagpur, West Bengal",
            ["Indian Institute of Technology Kanpur"] = "Kanpur, Uttar Pradesh",
            ["Indian Institute of Technology Roorkee"] = "Roorkee, Uttarakhand",
            ["Indian Institute of Technology Guwahati"] = "Guwahati, Assam",
            ["Anna University"] = "Chennai, Tamil Nadu",
            ["University of Mumbai"] = "Mumbai, Maharashtra",
            ["University of Pune"] = "Pune, Maharashtra",
            ["Bangalore University"] = "Bangalore, Karnataka",
            ["Visvesvaraya Technological University"] = "Belagavi, Karnataka",
            ["National Institute of Technology Tiruchirappalli"] = "Tiruchirappalli, Tamil Nadu",
            ["National Institute of Technology Karnataka"] = "Surathkal, Karnataka",
            ["National Institute of Technology Warangal"] = "Warangal, Telangana",
            ["Birla Institute of Technology and Science Pilani"] = "Pilani, Rajasthan",
            ["Birla Institute of Technology and Science Goa"] = "Goa, Goa",
            ["Vellore Institute of Technology"] = "Vellore, Tamil Nadu",
            ["Amrita Vishwa Vidyapeetham"] = "Coimbatore, Tamil Nadu",
            ["SRM Institute of Science and Technology"] = "Chennai, Tamil Nadu",
            ["SASTRA University"] = "Thanjavur, Tamil Nadu",
            ["Manipal Academy of Higher Education"] = "Manipal, Karnataka",
            ["Christ University"] = "Bangalore, Karnataka",
            ["Jain University"] = "Bangalore, Karnataka",
            
            // International Universities
            ["Harvard University"] = "Cambridge, Massachusetts, USA",
            ["Massachusetts Institute of Technology (MIT)"] = "Cambridge, Massachusetts, USA",
            ["Stanford University"] = "Stanford, California, USA",
            ["University of California Berkeley"] = "Berkeley, California, USA",
            ["Columbia University"] = "New York, New York, USA",
            ["University of Oxford"] = "Oxford, United Kingdom",
            ["University of Cambridge"] = "Cambridge, United Kingdom",
            ["Imperial College London"] = "London, United Kingdom",
            ["London School of Economics"] = "London, United Kingdom",
            ["University of Toronto"] = "Toronto, Ontario, Canada",
            ["University of British Columbia"] = "Vancouver, British Columbia, Canada",
            ["McGill University"] = "Montreal, Quebec, Canada",
            ["University of Melbourne"] = "Melbourne, Victoria, Australia",
            ["University of Sydney"] = "Sydney, New South Wales, Australia",
            ["Australian National University"] = "Canberra, Australian Capital Territory, Australia",
            ["National University of Singapore"] = "Singapore, Singapore",
            ["Nanyang Technological University"] = "Singapore, Singapore",
            ["University of Hong Kong"] = "Hong Kong",
            ["Tsinghua University"] = "Beijing, China",
            ["Peking University"] = "Beijing, China",
            ["University of Tokyo"] = "Tokyo, Japan",
            ["Kyoto University"] = "Kyoto, Japan",
            ["Seoul National University"] = "Seoul, South Korea",
            ["KAIST"] = "Daejeon, South Korea",
            ["Technical University of Munich"] = "Munich, Germany",
            ["ETH Zurich"] = "Zurich, Switzerland",
            ["Delft University of Technology"] = "Delft, Netherlands",
            
            ["Other"] = "Enter location manually"
        };

        public static Dictionary<string, string> Colleges = new()
        {
            // Junior Colleges (Intermediate)
            ["Narayana Junior College"] = "Multiple branches across India - enter exact branch location",
            ["Sri Chaitanya Junior College"] = "Multiple branches across India - enter exact branch location",
            ["Government Junior College"] = "Enter district and branch location",
            ["NRI Junior College"] = "Multiple branches across India - enter exact branch location",
            ["Vignan Junior College"] = "Guntur, Andhra Pradesh",
            ["Sri Chaitanya Junior College"] = "Hyderabad, Telangana",
            ["Bhashyam Junior College"] = "Guntur, Andhra Pradesh",
            ["Tirumala Junior College"] = "Tirupati, Andhra Pradesh",
            ["Sri Gayatri Junior College"] = "Rajahmundry, Andhra Pradesh",
            ["Chaitanya Junior College"] = "Vijayawada, Andhra Pradesh",
            ["Aditya Junior College"] = "Kakinada, Andhra Pradesh",
            ["Sri Venkateswara Junior College"] = "Tirupati, Andhra Pradesh",
            ["Government Junior College for Girls"] = "Hyderabad, Telangana",
            ["St. Ann's Junior College"] = "Secunderabad, Telangana",
            ["Little Flower Junior College"] = "Hyderabad, Telangana",
            
            // Degree Colleges
            ["St. Xavier's College"] = "Mumbai, Maharashtra",
            ["Loyola College"] = "Chennai, Tamil Nadu",
            ["Presidency College"] = "Chennai, Tamil Nadu",
            ["Lady Shri Ram College"] = "New Delhi, Delhi",
            ["Hindu College"] = "New Delhi, Delhi",
            ["Miranda House"] = "New Delhi, Delhi",
            ["Stephen's College"] = "New Delhi, Delhi",
            ["Hans Raj College"] = "New Delhi, Delhi",
            ["Fergusson College"] = "Pune, Maharashtra",
            ["Symbiosis College"] = "Pune, Maharashtra",
            ["Christ College"] = "Bangalore, Karnataka",
            ["Mount Carmel College"] = "Bangalore, Karnataka",
            ["St. Joseph's College"] = "Bangalore, Karnataka",
            ["Nizam College"] = "Hyderabad, Telangana",
            ["Aurora's Degree College"] = "Hyderabad, Telangana",
            ["Badruka College"] = "Hyderabad, Telangana",
            
            ["Other"] = "Enter location manually"
        };

        public static Dictionary<string, string> Schools = new()
        {
            ["Zilla Parishad High School"] = "Enter mandal and district location",
            ["Kendriya Vidyalaya"] = "Multiple branches across India - enter exact branch location",
            ["Delhi Public School"] = "Multiple branches across India - enter exact branch location",
            ["Bhashyam High School"] = "Guntur, Andhra Pradesh",
            ["Narayana High School"] = "Multiple branches across India - enter exact branch location",
            ["Sri Chaitanya High School"] = "Multiple branches across India - enter exact branch location",
            ["Oakridge International School"] = "Hyderabad, Telangana",
            ["International School of Hyderabad"] = "Hyderabad, Telangana",
            ["Chirec International School"] = "Hyderabad, Telangana",
            ["Glendale Academy"] = "Hyderabad, Telangana",
            ["Johnson Grammar School"] = "Hyderabad, Telangana",
            ["St. Ann's School"] = "Secunderabad, Telangana",
            ["St. Paul's School"] = "Hyderabad, Telangana",
            ["Little Flower High School"] = "Hyderabad, Telangana",
            ["All Saints High School"] = "Hyderabad, Telangana",
            ["St. George's Grammar School"] = "Hyderabad, Telangana",
            ["HPS Hyderabad Public School"] = "Hyderabad, Telangana",
            ["Gitanjali School"] = "Hyderabad, Telangana",
            ["Bharatiya Vidya Bhavan"] = "Hyderabad, Telangana",
            ["P. Obul Reddy Public School"] = "Hyderabad, Telangana",
            
            // International Schools
            ["The British School"] = "New Delhi, Delhi",
            ["American Embassy School"] = "New Delhi, Delhi",
            ["The International School of Bangalore"] = "Bangalore, Karnataka",
            ["Mallya Aditi International School"] = "Bangalore, Karnataka",
            ["Cathedral and John Connon School"] = "Mumbai, Maharashtra",
            ["Dhirubhai Ambani International School"] = "Mumbai, Maharashtra",
            ["Ecole Mondiale World School"] = "Mumbai, Maharashtra",
            
            ["Other"] = "Enter location manually"
        };

        public static string[] Boards = new[]
        {
            "CBSE - Central Board of Secondary Education",
            "ICSE - Indian Certificate of Secondary Education",
            "SSC - State Board",
            "IB - International Baccalaureate",
            "IGCSE - Cambridge International",
            "State Board of Secondary Education, Andhra Pradesh",
            "Telangana State Board of Intermediate Education",
            "Karnataka Secondary Education Examination Board",
            "Maharashtra State Board of Secondary and Higher Secondary Education",
            "Tamil Nadu State Board",
            "Kerala State Board",
            "Other"
        };

        public static string[] Degrees = new[]
        {
            // Undergraduate
            "B.Tech - Bachelor of Technology",
            "B.E - Bachelor of Engineering",
            "B.Sc - Bachelor of Science",
            "B.Com - Bachelor of Commerce",
            "B.A - Bachelor of Arts",
            "BBA - Bachelor of Business Administration",
            "BCA - Bachelor of Computer Applications",
            "B.Arch - Bachelor of Architecture",
            //"B.Pharm - Bachelor of Pharmacy",
            //"BDS - Bachelor of Dental Surgery",
            //"MBBS - Bachelor of Medicine, Bachelor of Surgery",
            //"B.V.Sc - Bachelor of Veterinary Science",
            "B.Ed - Bachelor of Education",
            //"LLB - Bachelor of Laws",
            
            // Postgraduate
            "M.Tech - Master of Technology",
            "M.E - Master of Engineering",
            "M.Sc - Master of Science",
            "M.Com - Master of Commerce",
            "M.A - Master of Arts",
            "MBA - Master of Business Administration",
            "MCA - Master of Computer Applications",
            "M.Arch - Master of Architecture",
            //"M.Pharm - Master of Pharmacy",
            //"MDS - Master of Dental Surgery",
            //"MD - Doctor of Medicine",
            //"MS - Master of Surgery",
            //"M.V.Sc - Master of Veterinary Science",
            "M.Ed - Master of Education",
            //"LLM - Master of Laws",
            //"PhD - Doctor of Philosophy",
            
            "Other"
        };

        public static string[] Specializations = new[]
        {
            // Engineering
            "Computer Science and Engineering",
            "Information Technology",
            "Electronics and Communication Engineering",
            "Electrical and Electronics Engineering",
            "Mechanical Engineering",
            "Civil Engineering",
            "Chemical Engineering",
            "Aerospace Engineering",
            "Biotechnology",
            "Bioinformatics",
            "Artificial Intelligence and Machine Learning",
            "Data Science",
            "Cyber Security",
            "Internet of Things",
            "Robotics",
            
            // Science
            "Physics",
            "Chemistry",
            "Mathematics",
            "Statistics",
            //"Botany",
            //"Zoology",
            //"Microbiology",
            //"Biotechnology",
            //"Environmental Science",
            //"Geology",
            
            // Commerce
            "Accounting and Finance",
            "Banking and Insurance",
            "Taxation",
            "Financial Management",
            "Marketing",
            "Human Resource Management",
            "International Business",
            "Supply Chain Management",
            "Economics",
            "Business Analytics",
            
            //// Arts
            //"English Literature",
            //"History",
            //"Political Science",
            //"Sociology",
            //"Psychology",
            //"Philosophy",
            //"Economics",
            //"Journalism and Mass Communication",
            //"Fine Arts",
            //"Performing Arts",
            
            //// Professional
            //"General Medicine",
            //"Surgery",
            //"Pediatrics",
            //"Gynecology and Obstetrics",
            //"Orthopedics",
            //"Cardiology",
            //"Neurology",
            //"Dermatology",
            //"Ophthalmology",
            //"ENT",
            
            "Other"
        };
    }
}
