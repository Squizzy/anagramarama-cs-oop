#!/usr/bin/env python3
"""
Wordlist Analyzer and Filter for Anagramarama
This script analyzes and filters wordlists to create optimal game content.
"""

import os
import csv
import requests
from collections import Counter, defaultdict
from typing import List, Dict, Set, Tuple
import unicodedata

class WordFrequencyLoader:
    """Handles loading and managing word frequency data."""
    
    FREQUENCY_URL = "https://raw.githubusercontent.com/hermitdave/FrequencyWords/master/content/2018/fr/fr_50k.txt"
    CACHE_FILE = "french_frequency.txt"
    
    @classmethod
    def load_frequency_data(cls, cache_dir: str) -> Dict[str, float]:
        """Load word frequency data, downloading if necessary."""
        cache_path = os.path.join(cache_dir, cls.CACHE_FILE)
        
        # Try to load from cache first
        if os.path.exists(cache_path):
            return cls._load_from_cache(cache_path)
            
        # Download and cache if not available
        return cls._download_and_cache(cache_path)
    
    @classmethod
    def _load_from_cache(cls, cache_path: str) -> Dict[str, float]:
        """Load frequency data from cache file."""
        frequencies = {}
        with open(cache_path, 'r', encoding='utf-8') as f:
            for line in f:
                if line.strip():
                    word, freq = line.strip().split(' ')
                    frequencies[word] = float(freq)
        return frequencies
    
    @classmethod
    def _download_and_cache(cls, cache_path: str) -> Dict[str, float]:
        """Download frequency data and save to cache."""
        try:
            response = requests.get(cls.FREQUENCY_URL)
            response.raise_for_status()
            
            frequencies = {}
            for line in response.text.splitlines():
                if line.strip():
                    word, freq = line.strip().split(' ')
                    frequencies[word] = float(freq)
            
            # Save to cache
            os.makedirs(os.path.dirname(cache_path), exist_ok=True)
            with open(cache_path, 'w', encoding='utf-8') as f:
                for word, freq in frequencies.items():
                    f.write(f"{word} {freq}\n")
                    
            return frequencies
            
        except Exception as e:
            print(f"Warning: Could not download frequency data: {e}")
            return {}

class WordlistAnalyzer:
    def __init__(self, input_file: str):
        """Initialize the analyzer with input file path."""
        self.input_file = input_file
        self.words = set()
        self.word_lengths: Dict[int, int] = defaultdict(int)
        self.anagram_groups: Dict[str, Set[str]] = defaultdict(set)
        self.word_frequencies: Dict[str, float] = {}
        self.min_word_length = 4
        self.max_word_length = 7
        
        # Load frequency data
        cache_dir = os.path.dirname(os.path.abspath(__file__))
        self.word_frequencies = WordFrequencyLoader.load_frequency_data(cache_dir)

    def remove_accents(self, word: str) -> str:
        """Remove accents from characters while keeping the base letter."""
        return ''.join(c for c in unicodedata.normalize('NFD', word)
                      if unicodedata.category(c) != 'Mn')

    def is_valid_word(self, word: str) -> bool:
        """Check if a word is valid for the game."""
        # Remove whitespace and convert to lowercase
        word = word.strip().lower()
        
        # Length check
        if not (self.min_word_length <= len(word) <= self.max_word_length):
            return False
            
        # Only allow letters
        if not all(c.isalpha() or c == '-' for c in word):
            return False
            
        return True

    def get_word_frequency(self, word: str) -> float:
        """Get the frequency score for a word."""
        # Try exact match first
        if word in self.word_frequencies:
            return self.word_frequencies[word]
        
        # Try without accents
        normalized = self.remove_accents(word)
        if normalized in self.word_frequencies:
            return self.word_frequencies[normalized]
            
        return 0.0

    def get_anagram_key(self, word: str) -> str:
        """Get sorted characters of word (anagram key)."""
        # Remove accents for anagram grouping
        word = self.remove_accents(word)
        return ''.join(sorted(word.lower()))

    def load_words(self):
        """Load and process words from input file and supplementary words."""
        try:
            # Load main wordlist
            with open(self.input_file, 'r', encoding='utf-8') as f:
                for line in f:
                    word = line.strip()
                    if self.is_valid_word(word):
                        self.words.add(word)
                        self.word_lengths[len(word)] += 1
                        
                        # Group by anagram key
                        key = self.get_anagram_key(word)
                        self.anagram_groups[key].add(word)

            # Load supplementary words
            supp_file = os.path.join(os.path.dirname(self.input_file), 'supplementary_words.txt')
            if os.path.exists(supp_file):
                with open(supp_file, 'r', encoding='utf-8') as f:
                    for line in f:
                        # Skip comments and empty lines
                        line = line.strip()
                        if not line or line.startswith('//'):
                            continue
                        
                        word = line.strip()
                        if self.is_valid_word(word):
                            self.words.add(word)
                            self.word_lengths[len(word)] += 1
                            
                            # Group by anagram key
                            key = self.get_anagram_key(word)
                            self.anagram_groups[key].add(word)
                print("Supplementary words loaded successfully")
                            
        except FileNotFoundError:
            print(f"Error: Could not find file {self.input_file}")
            return
        except Exception as e:
            print(f"Error loading words: {e}")
            return

    def analyze(self) -> Dict:
        """Analyze the wordlist and return statistics."""
        stats = {
            'total_words': len(self.words),
            'word_lengths': dict(self.word_lengths),
            'anagram_groups': {
                'total': len(self.anagram_groups),
                'distribution': defaultdict(int)
            },
            'frequency_stats': {
                'words_with_frequency': len([w for w in self.words if self.get_word_frequency(w) > 0]),
                'avg_frequency': sum(self.get_word_frequency(w) for w in self.words) / len(self.words) if self.words else 0
            }
        }
        
        # Count anagram group sizes
        for words in self.anagram_groups.values():
            stats['anagram_groups']['distribution'][len(words)] += 1
            
        return stats

    def filter_words(self, min_anagrams: int = 2, min_frequency: float = 0.0) -> Set[str]:
        """Filter words based on minimum number of anagrams and frequency."""
        filtered_words = set()
        
        for key, words in self.anagram_groups.items():
            if len(words) >= min_anagrams:
                # Filter by frequency if specified
                if min_frequency > 0:
                    words = {w for w in words if self.get_word_frequency(w) >= min_frequency}
                if words:  # Only add if we still have words after frequency filtering
                    filtered_words.update(words)
                
        return filtered_words

    def create_difficulty_lists(self) -> Dict[str, Set[str]]:
        """Create difficulty-based wordlists."""
        difficulty_lists = {
            'beginner': set(),    # 4-5 letters, 2+ anagrams, high frequency
            'intermediate': set(), # 5-6 letters, 3+ anagrams, medium frequency
            'advanced': set()      # 6-7 letters, 4+ anagrams, any frequency
        }
        
        # Calculate frequency thresholds
        frequencies = [self.get_word_frequency(w) for w in self.words]
        frequencies = [f for f in frequencies if f > 0]  # Remove zeros
        if frequencies:
            median_freq = sorted(frequencies)[len(frequencies)//2]
            high_freq = median_freq * 2
            med_freq = median_freq / 2
        else:
            high_freq = med_freq = 0
        
        for key, words in self.anagram_groups.items():
            word_len = len(next(iter(words)))  # Length of first word in group
            num_anagrams = len(words)
            
            # Filter words by frequency for each difficulty level
            if 4 <= word_len <= 5 and num_anagrams >= 2:
                high_freq_words = {w for w in words if self.get_word_frequency(w) >= high_freq}
                if high_freq_words:
                    difficulty_lists['beginner'].update(high_freq_words)
            elif 5 <= word_len <= 6 and num_anagrams >= 3:
                med_freq_words = {w for w in words if self.get_word_frequency(w) >= med_freq}
                if med_freq_words:
                    difficulty_lists['intermediate'].update(med_freq_words)
            elif 6 <= word_len <= 7 and num_anagrams >= 4:
                difficulty_lists['advanced'].update(words)
                
        return difficulty_lists

    def save_filtered_lists(self, output_dir: str):
        """Save filtered wordlists to files."""
        os.makedirs(output_dir, exist_ok=True)
        
        # Save difficulty-based lists
        difficulty_lists = self.create_difficulty_lists()
        for difficulty, words in difficulty_lists.items():
            output_file = os.path.join(output_dir, f'wordlist_{difficulty}.txt')
            with open(output_file, 'w', encoding='utf-8') as f:
                # Sort words by frequency, highest first
                sorted_words = sorted(words, key=lambda w: (-self.get_word_frequency(w), w))
                for word in sorted_words:
                    f.write(f"{word} # freq: {self.get_word_frequency(word):.6f}\n")

        # Save statistics
        stats = self.analyze()
        stats_file = os.path.join(output_dir, 'wordlist_stats.txt')
        with open(stats_file, 'w', encoding='utf-8') as f:
            f.write("Wordlist Statistics\n")
            f.write("==================\n\n")
            f.write(f"Total words: {stats['total_words']}\n")
            f.write(f"Words with frequency data: {stats['frequency_stats']['words_with_frequency']}\n")
            f.write(f"Average word frequency: {stats['frequency_stats']['avg_frequency']:.6f}\n\n")
            
            f.write("Word lengths distribution:\n")
            for length, count in sorted(stats['word_lengths'].items()):
                f.write(f"{length} letters: {count} words\n")
            
            f.write("\nAnagram groups distribution:\n")
            for size, count in sorted(stats['anagram_groups']['distribution'].items()):
                f.write(f"{size} anagrams: {count} groups\n")

def main():
    """Main function to run the analyzer."""
    # Get the directory of the current script
    current_dir = os.path.dirname(os.path.abspath(__file__))
    
    # Navigate to the French wordlist
    input_file = os.path.join(current_dir, '..', 'i18n', 'fr-FR', 'wordlist.txt')
    
    # Create output directory
    output_dir = os.path.join(current_dir, '..', 'i18n', 'fr-FR', 'filtered_wordlists')
    
    # Initialize and run analyzer
    analyzer = WordlistAnalyzer(input_file)
    analyzer.load_words()
    analyzer.save_filtered_lists(output_dir)
    
    print(f"Filtered wordlists have been created in {output_dir}")

if __name__ == "__main__":
    main() 